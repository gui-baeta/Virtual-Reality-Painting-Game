using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Slider = UnityEngine.UI.Slider;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

enum FixedAxis
{
    X = 0,
    Y = 1,
    Z = 2,
    None = 4,
}

public class LineProperties
{
    public List<Vector3> positions = new List<Vector3>();
    public Material material;
    public float startWidth = 0.04f;
    public float endWidth = 0.04f;

    public LineProperties(Vector3[] _positions, Material _material, float _startWidth, float _endWidth)
    {
        positions = new List<Vector3>(_positions);
        material = _material;
        startWidth = _startWidth;
        endWidth = _endWidth;
    }

    public GameObject createGameObj()
    {
        var gameObj = new GameObject();
        gameObj.AddComponent<LineRenderer>();

        var lineRdr = gameObj.GetComponent<LineRenderer>();
        lineRdr.positionCount = positions.Count;
        lineRdr.SetPositions(positions.ToArray());
        lineRdr.startWidth = startWidth;
        lineRdr.endWidth = endWidth;
        lineRdr.material = material;

        return gameObj;
    }

    public Mesh getBakedMesh()
    {
        var gameObj = createGameObj();

        Mesh mesh = new Mesh();
        gameObj.GetComponent<LineRenderer>().BakeMesh(mesh);

        GameObject.Destroy(gameObj);

        return mesh;
    }
}

public class Drawing : MonoBehaviour
{
    Vector3 lastPoint = Vector3.zero;

    public GameObject Prefab = null;

    public GameObject ParentPrefab = null;

    [SerializeField]
    private GameObject plane = null;

    public GameObject planePrefab = null;

    public Material _material;

    private int objectCount = 0;

    private int strokeCount = 0;

    public bool snapToPencil = false;

    public bool selectionMode = false;

    // private List<Vector3> snappingPoints = new List<Vector3>(
    // [
    //     new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 1, 0),
    //     new Vector3(0, -1, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1)
    // ]);

    private List<LineProperties> lineRendrs = new List<LineProperties>();
    private List<LineProperties> history = new List<LineProperties>();

    private FixedAxis fixedAxis = FixedAxis.None;

    public Slider widthSlider;

    private void OnGUI()
    {

    }

    private void Awake()
    {
        plane = Instantiate(planePrefab);
    }

    // Start is called before the first frame update
    void Start()
    {
        plane.SetActive(false);
        LineRenderer line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    public int GetParentCount()
    {
        return objectCount;
    }

    public void CreateObject(bool _val)
    {
        if (selectionMode) return;
        
        if (!lineRendrs.Any()) return; // If nothing was drawn yet, don't create an object!
        
        strokeCount = 0;
        objectCount += 1;
        GameObject parentObject = GameObject.Instantiate(ParentPrefab);
        parentObject.name = "Parent Object " + objectCount;
        int childCount = 1;
        List<GameObject> childObjs = new List<GameObject>();

        Vector3 maxPoint = Vector3.negativeInfinity;
        Vector3 minPoint = Vector3.positiveInfinity;

        foreach (var lineR in lineRendrs)
        {
            Mesh mesh = new Mesh();
            GameObject newChildObj = GameObject.Instantiate(Prefab);

            mesh = lineR.getBakedMesh();

            var triangles = mesh.triangles;
            var inv_tri = triangles.Reverse().ToList();
            var final_triangles = new List<int>(mesh.triangles);
            final_triangles.AddRange(inv_tri);
            mesh.triangles = final_triangles.ToArray();

            Vector3 offset = mesh.bounds.center;
            newChildObj.transform.position = mesh.bounds.center;

            newChildObj.GetComponent<MeshFilter>().sharedMesh = mesh;
            newChildObj.GetComponent<MeshCollider>().sharedMesh = mesh;

            List<Vector3> vertices = new List<Vector3>(newChildObj.GetComponent<MeshFilter>().sharedMesh.vertices);
            for(int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= offset;
            }
            newChildObj.GetComponent<MeshFilter>().sharedMesh.vertices = vertices.ToArray();
            newChildObj.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();

            newChildObj.name = "Child Object " + childCount;
            newChildObj.GetComponent<MeshRenderer>().material = new Material(lineR.material);
            childObjs.Add(newChildObj);

            maxPoint = Vector3.Max(maxPoint, newChildObj.GetComponent<MeshCollider>().bounds.max);
            minPoint = Vector3.Min(minPoint, newChildObj.GetComponent<MeshCollider>().bounds.min);

            childCount += 1;
        }
        parentObject.transform.position = (maxPoint + minPoint) * 0.5f;

        foreach (GameObject go in childObjs)
        {
            go.transform.parent = parentObject.transform;
        }
        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        parentObject.SetActive(true);

        parentObject.GetComponent<Rigidbody>().useGravity = true;
        parentObject.GetComponent<Rigidbody>().isKinematic = false;

        parentObject.AddComponent<Outline>().OutlineWidth = 25.0f;
        parentObject.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        parentObject.GetComponent<Outline>().OutlineColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        lineRendrs.Clear();
    }

    private Vector3 FixAxis(LineRenderer line, Vector3 point)
    {
        if (fixedAxis != FixedAxis.None && line.positionCount != 0)
        {
            point[(int)fixedAxis] = line.GetPosition(0)[(int)fixedAxis];
        }

        if (line.positionCount == 0)
        {
            plane.transform.position = point;
        }
        
        return point;
    }

    public void ReceivePoints(Vector3 point) {
        if (selectionMode) return;
        
        if (Vector3.Distance(point, lastPoint) > 0.01) {
            LineRenderer line = GetComponent<LineRenderer>();
            line.material = _material;
            line.startWidth = widthSlider.value;
            line.endWidth = widthSlider.value;
            point = FixAxis(line, point);
            if (fixedAxis == FixedAxis.None) {
                plane.SetActive(false);
                
            }
            else
            {
                plane.SetActive(true);
            }
            if (!snapToPencil || (snapToPencil && (line.positionCount == 0 || line.positionCount == 1)))
            {
                line.positionCount += 1;
            }
            line.SetPosition(line.positionCount - 1, point);
            lastPoint = point;
        }
    }

    public void offsetPlane(Vector3 point)
    {
        Vector2 offset2D = new Vector2(0, 0);
        Vector3 offset3D = point - lastPoint;
        var cameraPos = GameObject.Find("Main Camera").transform.position;
        var camDirection = 0.0f;
        if(fixedAxis == FixedAxis.X)
        {
            camDirection = cameraPos.x - point.x;
            offset2D = new Vector2(offset3D.y, offset3D.z);
        } 
        else if(fixedAxis == FixedAxis.Y)
        {
            camDirection = cameraPos.y - point.y;

            offset2D = new Vector2(offset3D.x, offset3D.z);
        }
        else
        {
            camDirection = cameraPos.z - point.z;

            offset2D = new Vector2(offset3D.x, offset3D.y);
        }

        if (camDirection >= 0.0f)
        {
            camDirection = 1.0f;
        }
        else
        {
            camDirection = -1.0f;
        }

        // var meshNormal = plane.GetComponent<MeshFilter>().sharedMesh.normals[0].normalized;
        // var normalDirection = meshNormal.x + meshNormal.y + meshNormal.z;
        plane.GetComponent<MeshRenderer>().material.mainTextureOffset = plane.GetComponent<MeshRenderer>().material.mainTextureOffset + offset2D * (0.33f * camDirection);
    }

    public void CreateStroke(bool _val)
    {
        if (selectionMode) return;
        
        history.Clear();
        strokeCount += 1;

        var strokeObj = new GameObject("Stroke " + strokeCount);
        LineRenderer line;
        line = gameObject.GetComponent<LineRenderer>();
        if (line == null)
        {
            throw new Exception("this shouldn't happen!");
        }
        Vector3[] positions = new Vector3[line.positionCount];
        line.GetPositions(positions);
        strokeObj.AddComponent<LineRenderer>();
        var newLine = strokeObj.GetComponent<LineRenderer>();
        line.material = new Material(_material);
        newLine.material = new Material(_material);
        newLine.positionCount = 0;

        var arr = new Vector3[line.positionCount];
        line.GetPositions(arr);
        newLine.positionCount = line.positionCount;

        newLine.SetPositions(arr);

        newLine.startWidth = line.startWidth;
        newLine.endWidth = line.endWidth;
        strokeObj.transform.parent = gameObject.transform;
        lineRendrs.Add(new LineProperties(arr, line.material, line.startWidth, line.endWidth));

        newLine.generateLightingData = true;

        
        line.positionCount = 0;
    }

    public void Undo(bool _val)
    {
        if (selectionMode) return;
        
        // TODO Check if user is drawing right now!!
        if (strokeCount != 0)
        {
            var strokeObj = GameObject.Find("Stroke " + strokeCount);
            GameObject.Destroy(strokeObj);

            strokeCount -= 1;
            // TODO History: move line properties to history from lineRendrs
            history.Add(lineRendrs[lineRendrs.Count - 1]);
            lineRendrs.RemoveAt(lineRendrs.Count - 1);

   
        }
    }

    public void Redo(bool _val)
    {
        if (selectionMode) return;
        
        if (history.Any())
        {
            // TODO History: Move line properties from history to lineRendrs
            lineRendrs.Add(history[history.Count - 1]);
            history.RemoveAt(history.Count - 1);
            
            strokeCount += 1;
            var childObj = lineRendrs[lineRendrs.Count - 1].createGameObj();
            childObj.transform.parent = gameObject.transform;
            childObj.name = "Stroke " + strokeCount;
        }
    }

    public void SetFixAxis(string axis)
    {
        if (selectionMode) return;
        
        if (fixedAxis == FixedAxis.X)
            plane.transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), -90.0f);
        if (fixedAxis == FixedAxis.Z)
            plane.transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f), -90.0f);
        
        switch (axis)
        {
            case "x":
                if (fixedAxis == FixedAxis.X) {
                    fixedAxis = FixedAxis.None;
                    plane.SetActive(false);
                } else { 
                    plane.SetActive(true);
                    plane.transform.Rotate(Vector3.forward, 90.0f);
                    fixedAxis = FixedAxis.X;
                }
                break;
            case "y":
                if (fixedAxis == FixedAxis.Y) {
                    fixedAxis = FixedAxis.None;
                    plane.SetActive(false);

                } else {
                    plane.SetActive(true);
                    fixedAxis = FixedAxis.Y;
                }
                break;
            case "z":
                if (fixedAxis == FixedAxis.Z) {
                    fixedAxis = FixedAxis.None;
                    plane.SetActive(false);

                } else {
                    plane.SetActive(true);
                    plane.transform.Rotate(Vector3.right, 90.0f);
                    fixedAxis = FixedAxis.Z;
                }

                break;
            default:
                fixedAxis = FixedAxis.None;
                break;
        }

    }

    public void InSelection(bool val)
    {
        selectionMode = val;
    }

    public void SnapToPencil(bool val)
    {
        if (selectionMode) return;
        snapToPencil = !snapToPencil;
    }
}
