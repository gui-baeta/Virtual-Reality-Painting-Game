using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public InputActionReference triggerUI;
    // Start is called before the first frame update
    void Start()
    {
        triggerUI.action.started += ToggleOnOff;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOnOff(InputAction.CallbackContext ctx)
    {
        try
        {
            if (!GameObject.Find("ObjectUI").activeInHierarchy)
            {
                gameObject.SetActive(!gameObject.activeInHierarchy);
            }
        }
        catch (NullReferenceException nre)
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }
}
