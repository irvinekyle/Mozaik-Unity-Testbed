using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CouchPotatoManager : MonoBehaviour {

    // Use this for initialization
    public string api_key = "d7e35c45861d4642b566c4d4d8b5643e";
    public string cp_baseurl = "http://localhost:5050/api/";

    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public void movieAdd(string imdbCode) {
        StartCoroutine(movieAddRequest(imdbCode));
    }
    IEnumerator movieAddRequest(string imdbCode) {
        //http://localhost:5050/api/d7e35c45861d4642b566c4d4d8b5643e/movie.add/?identifier=
        Debug.Log("Sending the follwoing:" + cp_baseurl + api_key + "/movie.add/?identifier=" + imdbCode);
        UnityWebRequest www = UnityWebRequest.Get(cp_baseurl + api_key + "/movie.add/?identifier=" + imdbCode);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log("GET result:" + www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
}
