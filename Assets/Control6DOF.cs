using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(ControllerConnectionHandler))]
public class Control6DOF : MonoBehaviour
{
    #region Private Variables
    private MLInputController _controller;
    private ControllerConnectionHandler _controllerConnectionHandler;
    private GameObject menuEl;
    private GameObject menuE2;
    private GameObject menuE3;
    private GameObject menuHolder;
    //public List<GameObject> menuObjects;
    private const float menuScale = 1f;
    private const float speed = 0.2f;
    
    #endregion

    #region Unity Methods
    void Start()
    {
        //menuObjects = new List<GameObject>();
        menuEl = GameObject.Find("el1");
        menuEl.transform.localScale = new Vector3(0f, 0f, 0f);
        menuE2 = GameObject.Find("el2");
        menuE2.transform.localScale = new Vector3(0f, 0f, 0f);
        menuE3 = GameObject.Find("el3");
        menuE3.transform.localScale = new Vector3(0f, 0f, 0f);
        menuHolder = GameObject.Find("MenuHolder");
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
        
    }
    #endregion

    private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        //MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (_controller != null && _controller.Id == controllerId &&
            button == MLInputControllerButton.Bumper)
        {
            // Demonstrate haptics using callbacks.
            _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceDown, MLInputControllerFeedbackIntensity.Medium);
            // Toggle UseCFUIDTransforms
            _controller.UseCFUIDTransforms = !_controller.UseCFUIDTransforms;
            Debug.Log("transforming position");
            menuEl.transform.Translate(0f,0f,0.07f, Space.Self);
            menuEl.transform.localScale = new Vector3(menuScale, menuScale, menuScale);
            //Vector3.Lerp(menuEl.transform.localScale, new Vector3(menuScale, menuScale, menuScale), curTime/Time.time)
            menuE2.transform.Translate(0f, 0f, 0.07f, Space.Self);
            menuE2.transform.localScale = new Vector3(menuScale, menuScale, menuScale);
            menuE3.transform.Translate(0f, 0f, 0.07f, Space.Self);
            menuE3.transform.localScale = new Vector3(menuScale, menuScale, menuScale);

        }
    }

    private void HandleOnButtonUp(byte controllerId, MLInputControllerButton button)
    {
        //MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (_controller != null && _controller.Id == controllerId &&
            button == MLInputControllerButton.Bumper)
        {
            // Demonstrate haptics using callbacks.
            _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.Medium);
            Debug.Log("resetting");
            menuEl.transform.Translate(0f, 0f, -0.07f, Space.Self);
            menuEl.transform.localScale = new Vector3(0f, 0f, 0f);
            menuE2.transform.Translate(0f, 0f, -0.07f, Space.Self);
            menuE2.transform.localScale = new Vector3(0f, 0f, 0f);
            menuE3.transform.Translate(0f, 0f, -0.07f, Space.Self);
            menuE3.transform.localScale = new Vector3(0f, 0f, 0f);
        }
    }

    private void HandleOnTriggerDown(byte controllerId, float value)
    {
        //MLInputController controller = _controllerConnectionHandler.ConnectedController;
        if (_controller != null && _controller.Id == controllerId)
        {
            MLInputControllerFeedbackIntensity intensity = (MLInputControllerFeedbackIntensity)((int)(value * 2.0f));
            _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click, intensity);
        }
    }
}


