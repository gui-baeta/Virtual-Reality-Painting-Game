using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Select : MonoBehaviour
{
    private bool isHovered = false;
    private bool isSelected = false;
    public InputActionReference aButton = null;


    // Start is called before the first frame update
    void Start()
    {
        aButton.action.started += SelectObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectObject(InputAction.CallbackContext ctx)
    {
        if (isHovered) {
            isSelected = !isSelected;

            GameObject selectedObjects = GameObject.Find("SelectedObjects");
            if (isSelected)
            {
                selectedObjects.GetComponent<SelectedObjects>().add(gameObject);
            }
            else
            {
                selectedObjects.GetComponent<SelectedObjects>().remove(gameObject);
            }
        }
    }

    public void Hovering()
    {
        isHovered = true;
    }

    public void stopHovering()
    {
        isHovered = false;
    }
}
