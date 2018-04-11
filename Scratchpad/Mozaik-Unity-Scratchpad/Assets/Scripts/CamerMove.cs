using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerMove : MonoBehaviour {

    public float speed = 2.0f;
    public float touchSpeed = 0.005f;
    private Vector3 cameraStartPos;
    private Vector3 trailerScreenStartPos;

    public Vector3 offset;
    private bool isMoving = false;
    public Camera mainCamera;
    public Transform trailerScreen;
    public float rotSPeed = 1.0f;
    public bool viewTrailerScreen;

    // Use this for initialization
    void Start () {
        cameraStartPos = transform.position; //Save camera postion on start
        trailerScreenStartPos = trailerScreen.transform.position;   //Start pos of the trailer screen
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.RightArrow) && !viewTrailerScreen) {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            trailerScreen.transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.LeftArrow) && !viewTrailerScreen) {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            trailerScreen.transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.touchCount == 1  && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            //Debug.Log("Camera move triggered: ");
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-touchDeltaPosition.x * touchSpeed, 0, 0);
            trailerScreen.transform.Translate(-touchDeltaPosition.x * touchSpeed, 0, 0);
        }
    }

    private void LateUpdate() {
        //Vector3 desiredPosition = trailerScreen.position + offset;
        //Vector3 desiredPostion = new Vector3(transform.position.x, transform.position.y+180, transform.position.z);
        
        if (viewTrailerScreen) {
            Quaternion desiredPostion = Quaternion.Euler(0, 180, 0);
            //Rotate the camera
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredPostion, Time.deltaTime);
        }
        else if (!viewTrailerScreen) {
            Quaternion desiredPostion = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredPostion, Time.deltaTime);
        }
    }

    public void resetCamera() {
        transform.position = cameraStartPos;
        trailerScreen.transform.position = trailerScreenStartPos;
        viewTrailerScreen = false;
    }
    /*
    public void rotateCamera(int numOfDegrees) {
        // call it with StartCoroutine:
        if (!isMoving) { // never start a new MoveObject while it's already running!
            StartCoroutine(MoveObject(mainCamera.transform, new Vector3(0,0,0), new Vector3(0, numOfDegrees, 0), new Quaternion(0,0,0,1), new Quaternion(0,numOfDegrees,0,-1), rotSPeed));
        }
    }
    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, float time) {
        isMoving = true; // MoveObject started
        float i = 0;
        float rate = 1 / time;
        while (i < 1) {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            thisTransform.rotation = Quaternion.Slerp(startRot, endRot, i);
            yield return 0;
        }
        isMoving = false; // MoveObject ended
    }
    */
}
