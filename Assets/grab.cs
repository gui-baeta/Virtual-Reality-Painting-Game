using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class grab : MonoBehaviour
{
    //public InputActionReference scale = null;
    public GameObject leftController = null;
    public GameObject rightController = null;
    public InputActionReference leftGripPressed;
    public float initialDistance = 0.0f;

    public bool scaling = false;
    public bool isGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {
        //scaleUp.action.started += onScaleUp;
        //scaleDown.action.started += onScaleDown;
        //scale.action.started += Scale;
        leftGripPressed.action.started += Scale;
        leftGripPressed.action.canceled += StopScale;

        leftController = GameObject.Find("LeftHand Controller");
        rightController = GameObject.Find("RightHand Controller");
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrabbed && scaling)
        {
            float currentDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            transform.localScale += new Vector3(1,1,1)*(currentDistance - initialDistance)*10;
            initialDistance = currentDistance;
        }
    }
    
    
    public void Grab(SelectEnterEventArgs args)
    {
        GameObject.Find("Strokes").GetComponent<Drawing>().selectionMode = true;
        isGrabbed = true;
    }

    public void Release(SelectExitEventArgs args)
    {
        GameObject.Find("Strokes").GetComponent<Drawing>().selectionMode = false;
        isGrabbed = false;
    }

    public void Scale(InputAction.CallbackContext ctx)
    {
        initialDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
        scaling = true;
    }

    public void StopScale(InputAction.CallbackContext ctx)
    {
        scaling = false;
    }
}
