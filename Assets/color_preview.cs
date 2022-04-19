using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class color_preview : MonoBehaviour
{
    private float _red = 0.914f;
    private float _green = 0.757f;
    private float _blue = 0.09f;

    public Material pencilMat;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void updateWidth(float width)
    {
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(width * 1000, 2, width * 1000);
    }

    public void updateRedColor(float red)
    {
        _red = red;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(_red, _green, _blue);
    }
    
    public void updateGreenColor(float green)
    {
        _green = green;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(_red, _green, _blue);
    }
    
    public void updateBlueColor(float blue)
    {
        _blue = blue;
        gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = new Color(_red, _green, _blue);
    }
}
