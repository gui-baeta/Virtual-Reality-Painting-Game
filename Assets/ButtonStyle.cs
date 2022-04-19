using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStyle : MonoBehaviour
{
    public bool pressed = false;
    public void OnGUI()
    {
        GUI.Label (new Rect (0,0,200,100), "Hi - I'm a label looking like a box", "box");
        pressed = GUI.Toggle(new Rect(10, 10, 100, 30), pressed, "Toggle me Daddy");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
