using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class TorrentManager : MonoBehaviour {


    public string API_KEY = "";
    public string movieIMDBtoSearchFor;
    public string magnetLink;

    private string urlSearchHead = "https://torrentapi.org/pubapi_v2.php?app_id=Mozaik&mode=search&search_imdb=";
    private string urlSearchTail = "&category=44&sort=seeders&token=";

    private PutioManager pio;

    // Use this for initialization
    void Start () {
        //Get new session key
        pio = GameObject.FindObjectOfType<PutioManager>();
        StartCoroutine(getSessionApiKey());
	}

    private IEnumerator getSessionApiKey() {
        //https://torrentapi.org/pubapi_v2.php?app_id=Mozaik&get_token=get_token
        Debug.Log("Sending the follwoing: https://torrentapi.org/pubapi_v2.php?app_id=Mozaik&get_token=get_token");
        UnityWebRequest www = UnityWebRequest.Get("https://torrentapi.org/pubapi_v2.php?app_id=Mozaik&get_token=get_token");
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            API_KEY = ProcessGetToken(www.downloadHandler.text);
        }
        //throw new NotImplementedException();
    }
    private string ProcessGetToken(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJson parsejson;
        parsejson = new parseJson();

        Debug.Log("Json Token key grabbed: " + jsonvale["token"]);
        return jsonvale["token"].ToString();

    }

    public string movieSearch(string imdbCode) {
        StartCoroutine(movieSearchRequest(imdbCode));
        return magnetLink;
    }
    IEnumerator movieSearchRequest(string imdbCode) {
        //https://torrentapi.org/pubapi_v2.php?app_id=Mozaik&mode=search&search_imdb=tt0167260&category=44&sort=seeders&token=96y7qmtwof
        Debug.Log("Sending the follwoing:" + urlSearchHead + imdbCode + urlSearchTail + API_KEY);
        UnityWebRequest www = UnityWebRequest.Get(urlSearchHead + imdbCode + urlSearchTail + API_KEY);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log("GET result:" + www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            ProcessMoviesResults(www.downloadHandler.text);
        }
    }
    private void ProcessMoviesResults(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJson parsejson;
        parsejson = new parseJson(); ;

        parsejson.filename = new ArrayList();
        parsejson.download = new ArrayList();

        for (int i = 0; i < jsonvale["torrent_results"].Count; i++) {

            parsejson.filename.Add(jsonvale["torrent_results"][i]["filename"].ToString());
            parsejson.download.Add(jsonvale["torrent_results"][i]["download"].ToString());

            Debug.Log("Json filename grabbed: " + jsonvale["torrent_results"][i]["filename"]);
            Debug.Log("Json magnet link grabbed: " + jsonvale["torrent_results"][i]["download"]);
        }
        pio.addMovieToPut(jsonvale["torrent_results"][0]["download"].ToString());

    }
    // Update is called once per frame
    void Update () {
		
	}
}
