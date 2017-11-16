using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovment : MonoBehaviour
{
    public Transform target;
    public GameObject upperPane;
    public GameObject lowerPane;

    public bool diffrentSign(float num1, float num2)
    {
        return !(num1 >= 0 && num2 >= 0 || num1 < 0 && num2 < 0);
    }

    public void setDistance(float distance)
    {
        Vector3 movment = (target.position - transform.position) * (transform.position - target.position).magnitude / distance;
        transform.Translate(transform.position + movment);
    }

    //float rotationYAxis = 0.0f;
    //float rotationXAxis = 0.0f;

    // Use this for initialization
    void Start() {
        transform.LookAt(target);//look at the target object i.e. planet

        Vector3 angles = transform.eulerAngles;
        //rotationYAxis = angles.y;
        //rotationXAxis = angles.x;
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }

    }

    //swipe drag variables
    public bool lastTwoTouch = false;
    private Vector2 fp;   //First touch position
    private Vector2 lp;   //Last touch position
    public float dragDistance = 50f;  //minimum distance for a swipe to be registered

    //pinch zoom variables
    public float perspectiveZoomSpeed = 0.001f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.001f;        // The rate of change of the orthographic size in orthographic mode.

    //rotation detection varibles
    bool rotating = false;
    float prevOfset = 0;
    Vector2 startVector;
    float rotGestureWidth = 40.0f;
    float rotAngleMinimum = 9.0f;
    float rotConst = 1.0f;

    // Update is called once per frame
    void Update()
    {
        //allow scrolling
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch

            if (lastTwoTouch)
            {
                fp = touch.position;
                lp = touch.position;
                lastTwoTouch = false;
            }
            else if (GameManager.RectToScreen(upperPane.GetComponent<RectTransform>()).Contains(touch.position) || GameManager.RectToScreen(lowerPane.GetComponent<RectTransform>()).Contains(touch.position))
            {//make sure its not in the GUI sections
                fp = new Vector2(0, 0);
                lp = new Vector2(0, 0);
            }
            else {
                if (touch.phase == TouchPhase.Began) //check for the first touch
                {
                    fp = touch.position;
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
                {
                    lp = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
                {
                    lp = touch.position;  //last touch position. Ommitted if you use list
                }

                if (Vector3.Distance(fp, lp) >= dragDistance)
                {
                    float constant = 0.1f;
                    transform.RotateAround(target.transform.position, Quaternion.Euler(transform.eulerAngles) * target.up, (lp.x - fp.x) * constant);
                    transform.RotateAround(target.transform.position, Quaternion.Euler(transform.eulerAngles) * target.right, (fp.y - lp.y) * constant);
                    fp = lp;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            lastTwoTouch = true;
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            if (GameManager.RectToScreen(upperPane.GetComponent<RectTransform>()).Contains(touchZero.position) || GameManager.RectToScreen(lowerPane.GetComponent<RectTransform>()).Contains(touchZero.position) ||
                GameManager.RectToScreen(upperPane.GetComponent<RectTransform>()).Contains(touchOne.position) || GameManager.RectToScreen(lowerPane.GetComponent<RectTransform>()).Contains(touchOne.position)) { }//make sure its not in the GUI sections
            else {
                //detect rotation gesture
                if (!rotating)
                {
                    startVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                    rotating = startVector.sqrMagnitude > rotGestureWidth * rotGestureWidth;
                    prevOfset = 0;
                }
                else {
                    Vector2 currVector = Input.GetTouch(1).position - Input.GetTouch(0).position;
                    float angleOffset = Vector2.Angle(startVector, currVector);
                    Vector3 LR = Vector3.Cross(startVector, currVector);


                    if (angleOffset > rotAngleMinimum)
                    {
                        if (LR.z > 0)
                        {
                            // Anticlockwise turn equal to angleOffset.
                            transform.Rotate(new Vector3(0, 0, (angleOffset-prevOfset)*rotConst));
                        }
                        else if (LR.z < 0)
                        {
                            // Clockwise turn equal to angleOffset.
                            transform.Rotate(new Vector3(0, 0, (angleOffset - prevOfset)*rotConst));
                        }
                    }
                    prevOfset = angleOffset;

                }

            }


            //detect pinch gesture
            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (Camera.main.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                Camera.main.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 18 and 109.
                Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 18f, 109f);
            }
        }
        else
        {
            rotating = false;
        }


    }

}
