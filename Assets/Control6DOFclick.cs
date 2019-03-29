using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(ControllerConnectionHandler))]
public class Control6DOFclick : MonoBehaviour
{
    #region Private Variables
    private MLInputController _controller;
    private ControllerConnectionHandler _controllerConnectionHandler;
    private const float menuScale = 0.8f;
    private const float selScale = 1f;
    private Vector3 upVal = new Vector3(0f,0f,0.08f);
    private Vector3 downVal = new Vector3(0f, 0f, -0.08f);
    private const float speed = 0.2f;
    private GameObject selectedOpt;

    private bool touchpad = false;
    private bool menuUp = false;

    // default index of selElIndex is the last element of the menuObjects and the 
    //last element should always be the "empty hand option"
    private int selElIndex;
    private int curIndex;

    public List<GameObject> menuObjects;

    private const float HALF_HOUR_IN_DEGREES = 15.0f;
    private const float DEGREES_PER_HOUR = 12.0f / 360.0f;

    private const int MIN_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock12);
    private const int MAX_LED_INDEX = (int)(MLInputControllerFeedbackPatternLED.Clock6And12);
    private const int LED_INDEX_DELTA = MAX_LED_INDEX - MIN_LED_INDEX;
    private int _lastLEDindex = -1;

    #endregion

    #region Unity Methods
    void Start()
    {
        menuObjects = new List<GameObject>
        {
            GameObject.Find("el0"),
            GameObject.Find("el1"),
            GameObject.Find("el2"),
            GameObject.Find("deselect")
        };
        selElIndex = menuObjects.Count - 1;
        curIndex = selElIndex;
        for (int i = 0; i < menuObjects.Count; i++)
        {
            menuObjects[i].transform.localScale = new Vector3(0f, 0f, 0f);
        }

        Debug.Log("Set all to 0");
        //Start receiving input by the Control
        MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _controllerConnectionHandler = GetComponent<ControllerConnectionHandler>();

        MLInput.OnControllerButtonUp += HandleOnButtonUp;
        MLInput.OnControllerButtonDown += HandleOnButtonDown;
        MLInput.OnTriggerDown += HandleOnTriggerDown;
    }
    void OnDestroy()
    {
        //Stop receiving input by the Control
        MLInput.Stop();
        if (MLInput.IsStarted)
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
            MLInput.OnControllerButtonUp -= HandleOnButtonUp;
        }
    }
    void Update()
    {
        //Attach the Beam GameObject to the Control
        transform.position = _controller.Position;
        transform.rotation = _controller.Orientation;
        if (selectedOpt != null)
        {
            selectedOpt.transform.rotation = _controller.Orientation;
            selectedOpt.transform.Rotate(-21.7f,0,0);
            selectedOpt.transform.position = _controller.Position;
            selectedOpt.transform.Translate(upVal, Space.Self);
        }
        UpdateLED();
    }
    #endregion

    private void UpdateLED()
    {
        if (!_controllerConnectionHandler.IsControllerValid())
        {
            return;
        }

        MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (controller.Touch1Active && touchpad)
        {
            // Get angle of touchpad position.
            float angle = -Vector2.SignedAngle(Vector2.up, controller.Touch1PosAndForce);
            if (angle < 0.0f)
            {
                angle += 360.0f;
            }

            // Get the correct hour and map it to [0,6]
            int index = (int)((angle + HALF_HOUR_IN_DEGREES) * DEGREES_PER_HOUR) % LED_INDEX_DELTA;

            // Pass from hour to MLInputControllerFeedbackPatternLED index  [0,6] -> [MAX_LED_INDEX, MIN_LED_INDEX + 1, ..., MAX_LED_INDEX - 1]
            index = (MAX_LED_INDEX + index > MAX_LED_INDEX) ? MIN_LED_INDEX + index : MAX_LED_INDEX;

            if (_lastLEDindex != index)
            {
                MLInputControllerTouchpadGestureDirection dir = controller.TouchpadGesture.Direction;
                Debug.Log(dir);
                if (dir.ToString().Equals("Left"))
                {
                    Debug.Log("left selected");
                    if(selElIndex <= 0)
                    {
                        selElIndex = menuObjects.Count - 2;
                    }
                    else
                    {
                        selElIndex--;
                    }
                    Debug.Log(selElIndex);
                    highlightEl();
                }
                else if (dir.ToString().Equals("Right"))
                {
                    if (selElIndex >= menuObjects.Count-2)
                    {
                        selElIndex = 0;
                    }
                    else
                    {
                        selElIndex++;
                    }
                    Debug.Log(selElIndex);
                    highlightEl();
                }
                else if (dir.ToString().Equals("Up"))
                {
                    selElIndex = menuObjects.Count - 1;
                    Debug.Log(selElIndex);
                    highlightEl();
                }
                else
                {
                    selElIndex = 1;
                    Debug.Log(selElIndex);
                    highlightEl();
                }
                // a duration of 0 means leave it on indefinitely
                controller.StartFeedbackPatternLED((MLInputControllerFeedbackPatternLED)index, MLInputControllerFeedbackColorLED.BrightCosmicPurple, 0);
                _lastLEDindex = index;
            }
        }
        else if (_lastLEDindex != -1)
        {
            controller.StopFeedbackPatternLED();
            _lastLEDindex = -1;
        }
    }

    private void highlightEl()
    {
        Debug.Log(selElIndex + "selected, changing from " + curIndex);
        GameObject selGB = menuObjects[selElIndex];
        GameObject curGB = menuObjects[curIndex];
        curGB.transform.localScale = new Vector3(menuScale, menuScale, menuScale);
        selGB.transform.localScale = new Vector3(selScale, selScale, selScale);
        curIndex = selElIndex;
        _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Bump, MLInputControllerFeedbackIntensity.Medium);
    }

    private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (menuUp)
        {
            menuUp = false;
            touchpad = false;
            //MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (_controller != null && _controller.Id == controllerId &&
                button == MLInputControllerButton.Bumper)
            {
                // Demonstrate haptics using callbacks.
                _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.Medium);
                Debug.Log("resetting");
                MenuScaleNMove("released");
                if (selElIndex != menuObjects.Count - 1)
                {
                    selectedOpt = Instantiate(menuObjects[selElIndex]);
                    float newScale = 0.05f * menuScale;
                    selectedOpt.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }
        else
        {
            menuUp = true;
            touchpad = true;
            if (selectedOpt != null)
            {
                selectedOpt.active = false;
            }
            Destroy(selectedOpt, 1.0f);
            //MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (_controller != null && _controller.Id == controllerId &&
                button == MLInputControllerButton.Bumper)
            {
                // Demonstrate haptics using callbacks.
                _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceDown, MLInputControllerFeedbackIntensity.Medium);
                // Toggle UseCFUIDTransforms
                _controller.UseCFUIDTransforms = !_controller.UseCFUIDTransforms;
                Debug.Log("transforming position");
                //Vector3.Lerp(menuEl.transform.localScale, new Vector3(menuScale, menuScale, menuScale), curTime/Time.time)
                MenuScaleNMove("pressed");
                highlightEl();
            }
        }
        
    }

    private void HandleOnButtonUp(byte controllerId, MLInputControllerButton button)
    {
        
    }

    private void HandleOnTriggerDown(byte controllerId, float value)
    {
        //MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (_controller != null && _controller.Id == controllerId)
        {
            MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(value * 2.0f));
            _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click, intensity);
            GameObject temp;
            if (selElIndex != menuObjects.Count - 1)
            {
                temp = Instantiate(menuObjects[selElIndex]);
                float newScale = 0.05f * menuScale;
                temp.transform.localScale = new Vector3(newScale, newScale, newScale);
                temp.transform.rotation = _controller.Orientation;
                temp.transform.Rotate(-21.7f, 0, 0);
                temp.transform.position = _controller.Position;
                temp.transform.Translate(upVal, Space.Self);
                temp.AddComponent<Rigidbody>();
                if(selElIndex == 0)
                {
                    temp.tag = "Plant";
                }else if (selElIndex == 1)
                {
                    temp.tag = "Meat";
                }
                else
                {

                }
                
            }
        }
    }

    //statuses are: "pressed" & "released"
    private void MenuScaleNMove(string status)
    {
        if (status.Equals("pressed"))
        {
            for (int i = 0; i < menuObjects.Count; i++)
            {
                menuObjects[i].transform.Translate(upVal, Space.Self);
                menuObjects[i].transform.localScale = new Vector3(menuScale, menuScale, menuScale);
            }
            
        }
        else
        {
            for (int i = 0; i < menuObjects.Count; i++)
            {
                menuObjects[i].transform.Translate(downVal, Space.Self);
                menuObjects[i].transform.localScale = new Vector3(0f, 0f, 0f);
            }
        }
    }
}


