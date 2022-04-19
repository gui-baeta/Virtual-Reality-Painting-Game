using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Pencil : MonoBehaviour
{

    public Text text;
    public UnityEvent<Vector3> onDraw;
    public UnityEvent<bool> onRelease;
    public UnityEvent<bool> onButtonAPress;
    public UnityEvent<bool> onButtonBPress;
    public UnityEvent<bool> onSnapToPencilClicked;

    public bool beingPressed;

    public InputActionReference buttonAPress;
    public InputActionReference buttonBPress;
    public InputActionReference triggerPressed;
    public InputActionReference snapToPencilClicked;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        buttonAPress.action.started += Redo;
        buttonBPress.action.started += Undo;

        triggerPressed.action.started += Press;
        triggerPressed.action.canceled += UnPress;
		snapToPencilClicked.action.started += SnapToPencil;
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        triggerPressed.action.started -= Press;
        buttonAPress.action.started -= Redo;
        buttonBPress.action.started -= Undo;
		snapToPencilClicked.action.started -= SnapToPencil;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // onSnapToPencilClicked.Invoke(true);
        // onDraw.Invoke(new Vector3(0.0f, 0.0f, 6.0f));
        // onDraw.Invoke(new Vector3(3.0f, 3.0f, 4.0f));
        // onDraw.Invoke(new Vector3(0.0f, 2.0f, 3.0f));
        // onDraw.Invoke(new Vector3(4.0f, 1.0f, 2.0f));
        // onRelease.Invoke(true);
        // // onSnapToPencilClicked.Invoke(true);
        //
        // GameObject.Find("Strokes").GetComponent<Drawing>().CreateObject(true);
        //
        //
        // onDraw.Invoke(new Vector3(13.0f, 0.0f, 10.0f));
        // onDraw.Invoke(new Vector3(13.0f, 0.5f, 12.0f));
        // onDraw.Invoke(new Vector3(10.0f, 1.0f, 12.0f));
        // onDraw.Invoke(new Vector3(10.0f, 1.5f, 12.0f));
        // onRelease.Invoke(true);
        //
        // GameObject.Find("Strokes").GetComponent<Drawing>().CreateObject(true);
        //
        // onDraw.Invoke(new Vector3(-13.0f, 0.0f, 10.0f));
        // onDraw.Invoke(new Vector3(-13.0f, 0.5f, 12.0f));
        // onDraw.Invoke(new Vector3(-10.0f, 1.0f, 12.0f));
        // onDraw.Invoke(new Vector3(-10.0f, 1.5f, 12.0f));
        // onRelease.Invoke(true);

        GameObject.Find("Strokes").GetComponent<Drawing>().CreateObject(true);


        // text.text = "Hello, World!";
    }

    // Update is called once per frame
    private void Update()
    {
        if (beingPressed) {
            onDraw.Invoke(transform.position);
        }

        //text.text = transform.position.ToString();
    }

    private void Press(InputAction.CallbackContext ctx)
    {
        var rightController = GameObject.Find("RightHand Controller");
        var color = rightController.GetComponent<LineRenderer>().materials[0].color;
        color[3] = 0.0f;
        rightController.GetComponent<LineRenderer>().materials[0].color = color;
        rightController.GetComponent<XRRayInteractor>().interactionLayerMask = 0;

        beingPressed = true;
    }

    private void UnPress(InputAction.CallbackContext ctx)
    {
        var rightController = GameObject.Find("RightHand Controller");
        var color = rightController.GetComponent<LineRenderer>().materials[0].color;
        color[3] = 1.0f;
        rightController.GetComponent<LineRenderer>().materials[0].color = color;
        rightController.GetComponent<XRRayInteractor>().interactionLayerMask = -5;

        beingPressed = false;
        onRelease.Invoke(true);
    }

    private void Undo(InputAction.CallbackContext ctx)
    {
        onButtonBPress.Invoke(true);
    }

    private void Redo(InputAction.CallbackContext ctx)
    {
        // onSnapToPencilClicked.Invoke(true);
        // onButtonAPress.Invoke(true);
    }

    private void SnapToPencil(InputAction.CallbackContext ctx)
    {
        onSnapToPencilClicked.Invoke(true);
    }
}
