using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestParent : MonoBehaviour
{
    public Material material = null;
    // Start is called before the first frame update
    void Start()
    {
        var childObj = new GameObject();
        childObj.transform.parent = gameObject.transform;
        childObj.AddComponent<MeshFilter>().AddComponent<MeshCollider>().AddComponent<MeshRenderer>();
        childObj.GetComponent<MeshRenderer>().material = material;
        childObj.GetComponent<MeshCollider>().convex = true;
        
        var mesh = new Mesh();
        gameObject.GetComponent<LineRenderer>().BakeMesh(mesh);
        childObj.GetComponent<MeshFilter>().sharedMesh = mesh;
        childObj.GetComponent<MeshCollider>().sharedMesh = mesh;
        Destroy(gameObject.GetComponent<LineRenderer>());
        gameObject.AddComponent<XRGrabInteractable>().interactionLayerMask = -5;
        //gameObject.GetComponent<XRGrabInteractable>().colliders.Add(childObj.GetComponent<MeshCollider>());
    }

    // Update is called once per frame
    void Update()
    {
    }
}
