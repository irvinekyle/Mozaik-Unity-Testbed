using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {
    public LayerMask touchInputMask;
    private List<GameObject> touchList = new List<GameObject>();
    public GameObject[] touchesOld;
    RaycastHit hit;


    Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame

    void Update () {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) {

            touchesOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchesOld);


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, touchInputMask)) {
                GameObject recipient = hit.transform.gameObject;
                touchList.Add(recipient);

                if (Input.GetMouseButtonDown(0)) {
                    recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("Mouse Input Handler Click:");
                }
                if (Input.GetMouseButtonUp(0)) {
                    recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("Mouse Input Handler Released:");
                }
                if (Input.GetMouseButton(0)) {
                    recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
                }
            }

            foreach (GameObject g in touchesOld) {
                if (!touchList.Contains(g)) {
                    g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
#endif

        if (Input.touchCount > 0) {

            touchesOld = new GameObject[touchList.Count];
            touchList.CopyTo(touchesOld);
            

            foreach (Touch touch in Input.touches) {
                Ray ray = cam.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit, touchInputMask)) {
                    GameObject recipient = hit.transform.gameObject;
                    touchList.Add(recipient);

                    if (touch.phase == TouchPhase.Began) {
                        recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Ended) {
                        recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
                        recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
                    }
                    if (touch.phase == TouchPhase.Canceled) {
                        recipient.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }

            foreach(GameObject g in touchesOld) {
                if (!touchList.Contains(g)) {
                    g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
	}
}
