using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KodiManager : MonoBehaviour {

    public string kodiIP = "http://192.168.1.45:8080/";
    public string testMoviePath = "https://api.put.io/v2/files/505277946/stream?oauth_token=RL52SYPN";

    private string kodiStreamRpcHeader = "jsonrpc?request={%20%22jsonrpc%22:%20%222.0%22,%20%22method%22:%20%22Player.Open%22,%20%22params%22:%20{%20%22item%22:%20{%20%22file%22:%20%22";
    private string kodiStreamRpcFooter = "%22%20}%20},%20%22id%22:%201%20}";

    //http://192.168.1.45:8080/jsonrpc?request={%20%22jsonrpc%22:%20%222.0%22,%20%22method%22:%20%22Player.Open%22,%20%22params%22:%20{%20%22item%22:%20{%20%22file%22:%20%22https://api.put.io/v2/files/505277946/stream?oauth_token=RL52SYPN%22%20}%20},%20%22id%22:%201%20}
   
    // Use this for initialization
    void Start () {
        // StartCoroutine(kodiStreanUrl(testMoviePath));
    }

    private IEnumerator kodiStreanUrl(string v) {
        //throw new NotImplementedException();

        //example string here
        Debug.Log("Sending the follwoing: " + kodiIP + kodiStreamRpcHeader + v + kodiStreamRpcFooter);
        UnityWebRequest www = UnityWebRequest.Get(kodiIP + kodiStreamRpcHeader + v + kodiStreamRpcFooter);
        yield return www.Send();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log("KODI GET result:" + www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    public void StartStream(string streamPath) {
        StartCoroutine(kodiStreanUrl(streamPath));
    }

    // Update is called once per frame
    void Update () {
		
	}
}
