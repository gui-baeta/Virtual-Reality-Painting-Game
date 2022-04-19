using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverOver()
    {
        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
    }
    
    public void HoverEnd()
    {
        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }
}
