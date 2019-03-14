using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StickyObject : MonoBehaviour
{
    private Camera _camera;
    //hold the objects initial orientation
    private Vector3 objectOrientationInCamera;
    private Quaternion objectRotationOffset;
    // Use this for initialization
    void Start()
    {
        _camera = Camera.main;
        if (_camera == null)
        {
            Debug.Log("Failed to get headpose");
            return;
        }
        //get the initial offsets for the object
        objectOrientationInCamera = transform.position;
        objectRotationOffset = transform.rotation;
        //adjust the y position to account for the y2 startpos of the camera
        objectOrientationInCamera.y -= 2;
    }

    // Update is called once per frame
    void Update()
    {
        //update the position of the object
        Vector3 posTo = _camera.transform.position + (_camera.transform.forward);
        posTo += objectOrientationInCamera;
        transform.position = Vector3.SlerpUnclamped(transform.position, posTo, Time.deltaTime * 5f);
        //update the rotation of the object
        Quaternion rotTo = Quaternion.LookRotation(transform.position - _camera.transform.position) * objectRotationOffset;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotTo, Time.deltaTime * 5f);
    }
}
