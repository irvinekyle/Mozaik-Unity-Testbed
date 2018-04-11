using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.Video;

public class PutioManager : MonoBehaviour {

    // Sample string for searching for playback file
    // https://put.io/v2/files/search/jazz from:”me,jack” ext:mp3
    // https://api.put.io/v2/files/list/?oauth_token=AY803QPM
    // https://put.io/v2/files/search/<query> from:"movies" type:video?oauth_token=AY803QPM
    // https://put.io/v2/files/search/star%20wars%20from:%22movies%22%20type:video?oauth_token=AY803QPM

    // Sample string for settinging up stream URL
    // https://api.put.io/v2/files/500970531/stream?oauth_token=RL52SYPN

    public string API_KEY = "AY803QPM";
    public Boolean searching = false;
    public VideoPlayer vp;
    private CamerMove camMove;
    public string movieToSearchFor;
    public string sessionFolderId;
    public string playbackFolderId;
    public string playbackFileId;
    public string magnetLink;
    public KodiManager km;

    private string urlSearchHead = "https://put.io/v2/files/search/";
    private string urlSearchTail = "%20type:video?oauth_token=";
    private string urlStreamHead = "https://api.put.io/v2/files/";
    private string urlStreamTail = "/stream?oauth_token=";


    // Use this for initialization
    void Start () {
        vp = FindObjectOfType<VideoPlayer>();
        camMove = GameObject.FindObjectOfType<CamerMove>();
        km = GameObject.FindObjectOfType<KodiManager>();

        // Create a new temp folder
        StartCoroutine(AddDirectoryToPut("Mozaik"));
    }

    // Update is called once per frame
    void Update () {
		
	}
    IEnumerator GetPutioFile(string movieToPlay) {
        //Example: https://put.io/v2/files/search/star%20wars%20from:%22movies%22%20type:video?oauth_token=AY803QPM
        Debug.Log("Sending the following:" + urlSearchHead + WWW.EscapeURL(movieToPlay) + urlSearchTail + API_KEY);
        UnityWebRequest www = UnityWebRequest.Get(urlSearchHead + WWW.EscapeURL(movieToPlay) + urlSearchTail + API_KEY);
        yield return www.Send();

        /*
        WWWForm formData = new WWWForm();
        formData.AddField("url", "magnet:?xt=urn:btih:24b6c69b027c67531d2947962a3ce7352292aa23&dn=Star.Wars.The.Last.Jedi.2017.1080p.BluRay.x264-SPARKS");
        formData.AddField("save_parent_id", "0");
        formData.AddField("extract", "False");
        formData.AddField("callback_url","None");
        formData.AddField("oauth_token", "J4HZ4TPW");

        UnityWebRequest www = UnityWebRequest.Post("https://api.put.io/v2/transfers/add", formData);
        */

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            new WaitForSeconds(10);
            StartCoroutine(ProcessPutioResults(www.downloadHandler.text));
        }
        yield return new WaitForSeconds(10);
    }

    // Create Blank directory
    IEnumerator AddDirectoryToPut(string name) {
        WWWForm formData = new WWWForm();
        formData.AddField("name", name);
        formData.AddField("parent_id", "0");
        formData.AddField("oauth_token", API_KEY);

        UnityWebRequest www = UnityWebRequest.Post("https://api.put.io/v2/files/create-folder", formData);
        yield return www.Send();
        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            StartCoroutine(GrabSessionFolderHandle(www.downloadHandler.text));
        }
    }
    // Grab Session folder ID
    IEnumerator GrabSessionFolderHandle(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);

        Debug.Log("jsonvale[file][id]: " + jsonvale["file"]["id"]);
        Debug.Log("Json id grabbed: " + jsonvale["file"]["id"]);
        Debug.Log("Setting new folder handle to following: " + jsonvale["file"]["id"]);

        sessionFolderId = jsonvale["file"]["id"].ToString();
        yield return 1;
    }
    // Create new transfer
    IEnumerator AddFileToPut(string url, string tempFolder) {

        WWWForm formData = new WWWForm();
        formData.AddField("url", url);
        formData.AddField("save_parent_id", tempFolder);
        formData.AddField("extract", "False");
        formData.AddField("callback_url", "None");
        formData.AddField("oauth_token", API_KEY);

        UnityWebRequest www = UnityWebRequest.Post("https://api.put.io/v2/transfers/add", formData);
        yield return www.Send();
        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
    // Get new files directory folder by querying the session folder
    IEnumerator GetSessionFolderList(string tempSessionFolder) {
        //Example: https://api.put.io/v2/files/list?parent_id=534337802&oauth_token=AY803QPM
        Debug.Log("Sending the following:" + "https://api.put.io/v2/files/list?parent_id=" + tempSessionFolder + "&oauth_token=" + API_KEY);
        UnityWebRequest www = UnityWebRequest.Get("https://api.put.io/v2/files/list?parent_id=" + tempSessionFolder + "&oauth_token=" + API_KEY);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log("Response from Directory query: " + www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            StartCoroutine(GrabPlaybackDirectoryHandle(www.downloadHandler.text));
        }
    }
    // Process the new playback directory folder
    IEnumerator GrabPlaybackDirectoryHandle(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);

        Debug.Log("jsonvale[files]: " + jsonvale["files"]);

        if (jsonvale.Keys.Contains("total") && jsonvale["total"].ToString() != "0") {
            for (int i = 0; i < jsonvale["files"].Count; i++) {

                playbackFolderId = jsonvale["files"][i]["id"].ToString();

                Debug.Log("Json play directory id grabbed: " + jsonvale["files"][i]["id"]);
                Debug.Log("Json playback name grabbed: " + jsonvale["files"][i]["name"]);
                StartCoroutine(GetPlaybackFolderList(playbackFolderId));
                yield return 1;
            }
        }
        else {
            yield return new WaitForSeconds(3);
            StartCoroutine(GetSessionFolderList(sessionFolderId));
        }
    }
    // Get playback folder list of files
    IEnumerator GetPlaybackFolderList(string playbackFolderId) {
        //Example: https://api.put.io/v2/files/list?parent_id=534337802&oauth_token=AY803QPM
        Debug.Log("Sending the following:" + "https://api.put.io/v2/files/list?parent_id=" + playbackFolderId + "&oauth_token=" + API_KEY);
        UnityWebRequest www = UnityWebRequest.Get("https://api.put.io/v2/files/list?parent_id=" + playbackFolderId + "&oauth_token=" + API_KEY);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log("Response from playback folder query: " + www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            StartCoroutine(GrabMovieFilePlayback(www.downloadHandler.text));
        }
    }
    // Searching for primary video file for payback
    IEnumerator GrabMovieFilePlayback(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        string tempType;

        Debug.Log("jsonvale[files]: " + jsonvale["files"].ToString());

        if (jsonvale.Keys.Contains("total") && jsonvale["total"].ToString() != "0") {
            for (int i = 0; i < jsonvale["files"].Count; i++) {
                // Check the file_type
                tempType = jsonvale["files"][i]["file_type"].ToString();
                if(tempType == "VIDEO") {
                    //
                    Debug.Log("Found video file:" + jsonvale["files"][i]["name"].ToString());
                    playbackFileId = jsonvale["files"][i]["id"].ToString();
                    Debug.Log("Setting streaming URL to following: " + urlStreamHead + playbackFileId + urlStreamTail + API_KEY);
                    yield return new WaitForSeconds(3);
                    vp.url = urlStreamHead + playbackFileId + urlStreamTail + API_KEY;
                    //km.StartStream(urlStreamHead + jsonvale["files"][0]["id"] + urlStreamTail + API_KEY);
                    km.StartStream(vp.url);
                    camMove.viewTrailerScreen = true;
                    break;
                }
            }
        }
        yield return 1;
    }
    // Delete playback directory on new search
    IEnumerator DeletePlaybackFolder(string pbFolder) {
        WWWForm formData = new WWWForm();
        formData.AddField("file_ids", pbFolder);
        formData.AddField("oauth_token", API_KEY);

        UnityWebRequest www = UnityWebRequest.Post("https://api.put.io/v2/files/delete", formData);
        yield return www.Send();
        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            StartCoroutine(GrabSessionFolderHandle(www.downloadHandler.text));
        }
    }





    IEnumerator ProcessPutioResults(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJson parsejson;
        parsejson = new parseJson(); ;

        parsejson.id = new ArrayList();
        parsejson.filename = new ArrayList();
        parsejson.screenshot = new ArrayList();

        Debug.Log("jsonvale[total]: " + jsonvale["total"]);

        if (jsonvale.Keys.Contains("total")) {
            if (jsonvale["total"].ToString() != "0") {
                for (int i = 0; i < jsonvale["files"].Count; i++) {

                    parsejson.id.Add(jsonvale["files"][i]["id"].ToString());

                    Debug.Log("Json id grabbed: " + jsonvale["files"][i]["id"]);
                    Debug.Log("Json name grabbed: " + jsonvale["files"][i]["name"]);
                    Debug.Log("Json screenshot grabbed: " + jsonvale["files"][i]["screenshot"]);
                }
                Debug.Log("Setting streaming URL to following: " + urlStreamHead + jsonvale["files"][0]["id"] + urlStreamTail + API_KEY);
                vp.url = urlStreamHead + jsonvale["files"][0]["id"] + urlStreamTail + API_KEY;
                //km.StartStream(urlStreamHead + jsonvale["files"][0]["id"] + urlStreamTail + API_KEY);
                camMove.viewTrailerScreen = true;

                //Resets flags
                movieToSearchFor = "";
                searching = false;
                yield return new WaitForSeconds(10);
            }
            else {
                Debug.Log("No files found yet: ");
                yield return new WaitForSeconds(2);
                searching = false;
            }
        }
    }

    public void FixedUpdate() {
        if(!searching && movieToSearchFor.Length > 0) {
            searching = true;
            //StartCoroutine(GetPutioFile(movieToSearchFor));
        }
    }
    public void pollForMovie(string movieTitle) {
        movieToSearchFor = movieTitle;
    }
    public void addMovieToPut(string magnetLink) {
        // Add new file to new folder
        StartCoroutine(AddFileToPut(magnetLink, sessionFolderId));
        // Poll for new directory = Transfer is completed
        StartCoroutine(GetSessionFolderList(sessionFolderId));
    }
    public void clearSessionFolder() {
        StartCoroutine(DeletePlaybackFolder(playbackFolderId));
    }
}
