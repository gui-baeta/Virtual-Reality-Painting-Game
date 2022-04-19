using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectedObjects : MonoBehaviour
{
    public UnityEvent<bool> selectionMode;

    public List<GameObject> objs = new List<GameObject>();
    
    public GameObject UI = null;
    public GameObject objectUI = null;

    public GameObject parentPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        objectUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ToggleGravity(bool gravity) {
        foreach (var obj in objs)
        {
            obj.GetComponent<Rigidbody>().useGravity = gravity;
        }
    }
    public void ToggleRigid(bool rigid) {
        foreach (var obj in objs)
        {
            obj.GetComponent<Rigidbody>().isKinematic = rigid;
        }
    }

    public void UpdateColorRed(float red)
    {
        foreach (var obj in objs)
        {
            foreach (Transform child in obj.transform)
            {
                var color = child.gameObject.GetComponent<MeshRenderer>().material.color;
                child.gameObject.GetComponent<MeshRenderer>().material.color = new Color(red, color.g, color.b);
            }
        }
    }
    
    public void UpdateColorGreen(float green)
    {
        foreach (var obj in objs)
        {
            foreach (Transform child in obj.transform)
            {
                var color = child.gameObject.GetComponent<MeshRenderer>().material.color;
                child.gameObject.GetComponent<MeshRenderer>().material.color = new Color(color.r, green, color.b);
            }
        }
    }
    
    public void UpdateColorBlue(float blue)
    {
        foreach (var obj in objs)
        {
            foreach (Transform child in obj.transform)
            {
                var color = child.gameObject.GetComponent<MeshRenderer>().material.color;
                child.gameObject.GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, blue);
            }
        }
    }

    public void add(GameObject obj)
    {
        obj.gameObject.GetComponent<Outline>().OutlineColor = Color.magenta;
        objs.Add(obj);
        if(objs.Count == 1)
        {
            UI.SetActive(false);
            objectUI.SetActive(true);

            selectionMode.Invoke(true);
            var rightController = GameObject.Find("RightHand Controller");
            var invGradient = new Gradient();
            var colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.grey;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.grey;
            colorKey[1].time = 1.0f;
            rightController.GetComponent<XRInteractorLineVisual>().invalidColorGradient = invGradient;


            GameObject.Find("Gravity").GetComponent<Toggle>().isOn = obj.GetComponent<Rigidbody>().useGravity;
            GameObject.Find("Rigid").GetComponent<Toggle>().isOn = obj.GetComponent<Rigidbody>().isKinematic;

        } else if(objs.Count == 2)
        {
            GameObject groupBtn = GameObject.Find("Group");
            GameObject placeHolderBtn = GameObject.Find("PlaceHolder");

            groupBtn.SetActive(true);
            //placeHolderBtn.SetActive(true);

            groupBtn.GetComponent<Button>().onClick.AddListener(groupObjects);
        }
        
        if (objs.Count >= 2)
        {
            GameObject.Find("Delete Objects").GetComponentInChildren<Text>().text = "Delete Selected Objects";
        }
    }

    public void remove(GameObject obj)
    {
        if (objs.Count <= 1)
        {
            GameObject.Find("Delete Objects").GetComponentInChildren<Text>().text = "Delete Selected Object";
        }
        
        obj.gameObject.GetComponent<Outline>().OutlineColor = new Color(0.0f,0.0f,0.0f,0.0f);
        objs.Remove(obj);
        if(objs.Count == 0)
        {
            selectionMode.Invoke(false);
            var rightController = GameObject.Find("RightHand Controller");
            var invGradient = new Gradient();
            var colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.red;
            colorKey[1].time = 1.0f;
            rightController.GetComponent<XRInteractorLineVisual>().invalidColorGradient = invGradient;

            objectUI.SetActive(false);
            UI.SetActive(true);
        }
    }

    public void groupObjects()
    {
        print("=================================================================");
        if (objs.Count <= 1) return;
        
        GameObject newParent = GameObject.Instantiate(parentPrefab);
        newParent.name = "Parent Object " + GameObject.Find("Strokes").GetComponent<Drawing>().GetParentCount();
        for (var i = objs.Count - 1; i >= 0; i-=1)
        {
            objs[i].gameObject.GetComponent<Outline>().OutlineColor = new Color(0.0f,0.0f,0.0f,0.0f);
            var childCount = 1;

            print("Parent " + objs[i].name +" that has " + objs[i].transform.childCount + "children");
            var childrenList = new List<Transform>(objs[i].transform.GetComponentsInChildren<Transform>());
            foreach(var child in childrenList)
            {
                GameObject curr = child.gameObject;

                if (curr.name.Contains("attachment")) continue;
                
                print("\tChild: " + curr.name);
                
                if(curr.name != objs[i].name)
                {
                    curr.transform.parent = newParent.transform;
                }

                var nameVec = new List<String>(curr.name.Split());
                nameVec.RemoveAt(nameVec.Count - 1);
                curr.name = String.Join(" ", nameVec) + " " + childCount;
                
                childCount += 1;
            } 
            GameObject.Destroy(objs[i]);
        }
        
        selectionMode.Invoke(false);
        objs.Clear();
        objectUI.SetActive(false);
        UI.SetActive(true);
        
        newParent.AddComponent<Outline>().OutlineWidth = 25.0f;
        newParent.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
        newParent.GetComponent<Outline>().OutlineColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        newParent.SetActive(true);
        print(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
    }

    public void DeleteObjects()
    {
        foreach (var obj in objs)
        {
            print(obj.name);
        }
        for (var i = objs.Count - 1; i >= 0 ; i-=1) GameObject.Destroy(GameObject.Find(objs[i].name));
        
        objs.Clear();
        if(objs.Count == 0)
        {
            selectionMode.Invoke(false);
            var rightController = GameObject.Find("RightHand Controller");
            var invGradient = new Gradient();
            var colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.red;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.red;
            colorKey[1].time = 1.0f;
            rightController.GetComponent<XRInteractorLineVisual>().invalidColorGradient = invGradient;

            objectUI.SetActive(false);
            UI.SetActive(true);
        }
    }

    public void CancelSelection()
    {
        for (var i = objs.Count - 1; i >= 0; i -= 1) remove(objs[i]);
    }

    // TODO
    // public void SelectAll()
    // {
    //     // for (var i = 0; i < GameObject.Find("Strokes").GetComponent<Drawing>().GetParentCount(); i -= 1)
    //     // {
    //     //     
    //     // }
    //     // GameObject.Find("Parent Object" + )
    // }
}
