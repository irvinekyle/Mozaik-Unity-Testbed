using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour {

    // Use this for initialization

    public string sendString;
    private WebSocket w = new WebSocket(new Uri("ws://192.168.1.54:12345"));


    IEnumerator Start() {
            yield return StartCoroutine(w.Connect());
            w.Close();
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.X))
        {
            //do stuff
            Debug.Log("X triggered: running websocket");
            w.SendString(sendString);
            while (true) {
                string reply = w.RecvString();
                if (reply != null) {
                    Debug.Log("Received: " + reply);
                }
                if (w.error != null) {
                    //Debug.LogError ("Error: "+w.error);
                    break;
                }
                else {
                    return;
                }
            }
        }
    }
}
