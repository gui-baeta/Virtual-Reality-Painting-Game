using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeObjUI : MonoBehaviour
{
    void Awake()
    {
        GameObject camera = GameObject.Find("Main Camera");
        gameObject.GetComponent<Canvas>().worldCamera = camera.GetComponent<Camera>();

        var selectedObjs = GameObject.Find("SelectedObjects");
        GameObject.Find("Slider Red").GetComponent<Slider>().onValueChanged.AddListener(selectedObjs.GetComponent<SelectedObjects>().UpdateColorRed);
        GameObject.Find("Slider Green").GetComponent<Slider>().onValueChanged.AddListener(selectedObjs.GetComponent<SelectedObjects>().UpdateColorGreen);
        GameObject.Find("Slider Blue").GetComponent<Slider>().onValueChanged.AddListener(selectedObjs.GetComponent<SelectedObjects>().UpdateColorBlue);
        
        GameObject.Find("Gravity").GetComponent<Toggle>().onValueChanged.AddListener(selectedObjs.GetComponent<SelectedObjects>().ToggleGravity);
        GameObject.Find("Rigid").GetComponent<Toggle>().onValueChanged.AddListener(selectedObjs.GetComponent<SelectedObjects>().ToggleRigid);
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
