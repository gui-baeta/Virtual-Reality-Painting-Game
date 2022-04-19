using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stroky : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var line = gameObject.GetComponent<LineRenderer>();
        var mesh = new Mesh();
        line.BakeMesh(mesh);
        gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.GetComponent<LineRenderer>().positionCount = 0;
        Destroy(gameObject.GetComponent<LineRenderer>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
