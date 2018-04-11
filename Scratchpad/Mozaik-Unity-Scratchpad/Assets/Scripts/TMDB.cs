using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.UI;

public class parseJson {
    public ArrayList title;
    public ArrayList id;
    public ArrayList but_title;
    public ArrayList but_image;
    public ArrayList filename;
    public ArrayList screenshot;
    public ArrayList download;
}

[System.Serializable]
public class TMDB : Poster {


    private string APIkey = "13081b6134223a4e3985c10f10ec49b6";
    private string urlSearchHead = "https://api.themoviedb.org/3/search/movie?api_key=";
    private string urlImdbHead = "https://api.themoviedb.org/3/movie/";

    public InputField urlQuery;

    public float xOffset = 2;
    public float xStart = 0;
    public float speed = 1;
    public LayerMask touchInputMask;

    private GameObject[] postersObjs;
    private Poster newPosterData;
    private Camera cam;
    private RaycastHit hit;

    private PutioManager pm;

    // Update is called once per frame
    void Update () {
        
    }
    void Start() {
        //StartCoroutine(GetText(runSearch));
        cam = FindObjectOfType<Camera>();
        pm = FindObjectOfType<PutioManager>();
    }

    IEnumerator GetMoviesResults(string searchQuery) {
        Debug.Log("Sending the follwoing:" + urlSearchHead + APIkey + "&language=en-US&query=" + searchQuery + "&page = 1 & include_adult = false");
        UnityWebRequest www = UnityWebRequest.Get(urlSearchHead + APIkey + "&language=en-US&query=" + searchQuery + "&page = 1 & include_adult = false");
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            ProcessMoviesResults(www.downloadHandler.text);
        }
    }
    IEnumerator GetMovieImdb(string movieCode) {
        //https://api.themoviedb.org/3/movie/31?api_key=<<api_key>>&language=en-US
        Debug.Log("Sending the follwoing:" + urlImdbHead + movieCode + "?api_key=" + APIkey + "&language=en-US");
        UnityWebRequest www = UnityWebRequest.Get(urlImdbHead + movieCode + "?api_key=" + APIkey + "&language=en-US");
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            ProcessImdbResults(www.downloadHandler.text);
        }
    }
    IEnumerator RenderThumbnail(string tmdbCode, string titleThumb, string urlThumb) {
        WWW www = new WWW("https://image.tmdb.org/t/p/w500" + urlThumb);
        Debug.Log("Rendering the following: " + "https://image.tmdb.org/t/p/w500" + urlThumb);
        yield return www;

        //Adding the class in definition

        GameObject poster = GameObject.CreatePrimitive(PrimitiveType.Cube);
        poster.AddComponent<Poster>();
        newPosterData = poster.GetComponent<Poster>();
        poster.tag = "Poster";
        poster.layer = LayerMask.NameToLayer("Touchable");
        newPosterData.tmdbCode = tmdbCode;
        newPosterData.thumbnail = "https://image.tmdb.org/t/p/w500" + urlThumb;
        newPosterData.title = titleThumb;


        poster.transform.localScale = new Vector3(1, 1.52f, 0.02f);
        poster.transform.position = new Vector3(xStart, 1, 0);
        poster.transform.localRotation *= Quaternion.Euler(0, 0, 180);
        xStart += xOffset;

        Renderer renderer = poster.GetComponent<Renderer>();
        renderer.material.mainTexture = www.texture;
        renderer.material.shader = Shader.Find("Standard (Specular setup)");
        renderer.material.SetColor("_SpecColor", Color.black);
    }
    private string ProcessImdbResults(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJson parsejson;
        parsejson = new parseJson();

        Debug.Log("Json imdb Tag grabbed: " + jsonvale["imdb_id"]);
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            GameObject recipient = hit.transform.gameObject;
            Poster posterObj = recipient.GetComponent<Poster>();

            Debug.Log("Running IMDB yield returns: " + jsonvale["imdb_id"].ToString());
            Debug.Log("posterObj=" + recipient.gameObject.ToString());
            Debug.Log("posterObj Title=" + posterObj.title);
            posterObj.ImdbReturn(jsonvale["imdb_id"].ToString());
        }
        else {
            Debug.LogWarning("Raycast check failed!!!");
        }
        return jsonvale["imdb_id"].ToString();

    }
    private void ProcessMoviesResults(string jsonString) {
        JsonData jsonvale = JsonMapper.ToObject(jsonString);
        parseJson parsejson;
        parsejson = new parseJson(); ;

        parsejson.id = new ArrayList();
        parsejson.but_title = new ArrayList();
        parsejson.but_image = new ArrayList();

        for (int i = 0; i < jsonvale["results"].Count; i++) {

            parsejson.id.Add(jsonvale["results"][i]["id"].ToString());
            parsejson.but_title.Add(jsonvale["results"][i]["title"].ToString());
            parsejson.but_image.Add(jsonvale["results"][i]["poster_path"].ToString());

            Debug.Log("Json id grabbed: " + jsonvale["results"][i]["id"]);
            Debug.Log("Json title grabbed: " + jsonvale["results"][i]["title"]);
            Debug.Log("Json image grabbed: " + jsonvale["results"][i]["poster_path"]);

            StartCoroutine(RenderThumbnail(jsonvale["results"][i]["id"].ToString(),
                                            jsonvale["results"][i]["title"].ToString(),
                                            jsonvale["results"][i]["poster_path"].ToString()));
        }
    }
    public void runSearchClick() {
        DestroyAllPosterObjs();     //Clean up old posters
        pm.clearSessionFolder();    //Clean up temp session folder in Put
        StartCoroutine(GetMoviesResults(urlQuery.text));
    }
    public void runImdbQueryClick(string codeToQuery) {
        Debug.Log("Running IMDB lookup for: " + codeToQuery);
        StartCoroutine(GetMovieImdb(codeToQuery)).ToString();
    }
    void DestroyAllPosterObjs() {
        postersObjs = GameObject.FindGameObjectsWithTag("Poster");
        xStart = 0; //resest xPosition
        for (var i = 0; i < postersObjs.Length; i++) {
            Destroy(postersObjs[i]);
        }
    }
}
