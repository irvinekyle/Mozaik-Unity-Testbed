using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poster : MonoBehaviour {

    public string title;
    public string thumbnail;
    public string tmdbCode;

    private Poster posterObj;
    public TMDB tmdb;
    private CouchPotatoManager cp;
    private TorrentManager tm;
    private PutioManager pio;
    private CamerMove camMove;

    private void Start() {
        tmdb = GameObject.FindObjectOfType<TMDB>();
        camMove = GameObject.FindObjectOfType<CamerMove>();
        posterObj = GetComponent<Poster>();
        cp = GameObject.FindObjectOfType<CouchPotatoManager>();
        tm = GameObject.FindObjectOfType<TorrentManager>();
        pio = GameObject.FindObjectOfType<PutioManager>();
    }
  
	void OnTouchDown() {
        Debug.Log("TouchDown:");
        Debug.Log(posterObj.title);
        string ImdbQuery = posterObj.tmdbCode;
        Debug.Log("TMDB Code to look up is: " + ImdbQuery);

        tmdb.runImdbQueryClick(ImdbQuery);
    }
    void OnTouchUp() {
        Debug.Log("TouchRelease:");
    }
    void OnTouchStay() {
        Debug.Log("TouchStay:");
    }
    void OnTouchExit() {
        Debug.Log("TouchExit:");
    }
    public void  ImdbReturn(string newImdbTag) {
        Debug.Log("Recieved newImdbTag: " + newImdbTag);
        //cp.movieAdd(newImdbTag);
        tm.movieSearch(newImdbTag);
        pio.pollForMovie(posterObj.title);
    }
    void SetTitle(string newTitle) {
        this.title = newTitle;
    }
    public void MovieReady() {
        // Roatate the camera to view the screen
    }
}
