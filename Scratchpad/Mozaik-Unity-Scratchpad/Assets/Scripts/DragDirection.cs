using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDirection : MonoBehaviour {
    public bool swiping;

    public float minSwipeDistance;
    public float errorRange;

    public SwipeDirection direction = SwipeDirection.None;

    public enum SwipeDirection { Right, Left, Up, Down, None }

    private Touch initialTouch;

    void Start() {
        Input.multiTouchEnabled = true;
    }

    void Update() {
        if (Input.touchCount <= 0)
            return;

        foreach (var touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                initialTouch = touch;
            }
            else if (touch.phase == TouchPhase.Moved) {
                var deltaX = touch.position.x - initialTouch.position.x; //greater than 0 is right and less than zero is left
                var deltaY = touch.position.y - initialTouch.position.y; //greater than 0 is up and less than zero is down
                var swipeDistance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);

                if (swipeDistance > minSwipeDistance && (Mathf.Abs(deltaX) > 0 || Mathf.Abs(deltaY) > 0)) {
                    swiping = true;

                    //Debug.Log("Swipe code triggered with: detlaX=" + deltaX + " deltaY=" + deltaY);
                    CalculateSwipeDirection(deltaX, deltaY);
                }
            }
            else if (touch.phase == TouchPhase.Ended) {
                initialTouch = new Touch();
                swiping = false;
                direction = SwipeDirection.None;
            }
            else if (touch.phase == TouchPhase.Canceled) {
                initialTouch = new Touch();
                swiping = false;
                direction = SwipeDirection.None;
            }
        }
    }

    void CalculateSwipeDirection(float deltaX, float deltaY) {
        bool isHorizontalSwipe = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

        // horizontal swipe
        if (isHorizontalSwipe && Mathf.Abs(deltaY) <= errorRange) {
            //right
            if (deltaX > 0)
                direction = SwipeDirection.Right;
            //left
            else if (deltaX < 0)
                direction = SwipeDirection.Left;

            Debug.Log("Swipe Direction is: " + direction);
        }
        //vertical swipe
        else if (!isHorizontalSwipe && Mathf.Abs(deltaX) <= errorRange) {
            //up
            if (deltaY > 0)
                direction = SwipeDirection.Up;
            //down
            else if (deltaY < 0)
                direction = SwipeDirection.Down;
        }
        //diagonal swipe
        else {
            swiping = false;
        }
    }
}
    
    /*
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("Press position + " + eventData.pressPosition);
        Debug.Log("End position + " + eventData.position);
        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        Debug.Log("norm + " + dragVectorDirection);
        GetDragDirection(dragVectorDirection);
    }
    private enum DraggedDirection {
        Up,
        Down,
        Right,
        Left
    }
    private DraggedDirection GetDragDirection(Vector3 dragVector) {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DraggedDirection draggedDir;
        if (positiveX > positiveY) {
            draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else {
            draggedDir = (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
        Debug.Log(draggedDir);
        return draggedDir;
    }*/
