using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    private void Awake()
    {
        base.Awake();
        // Create an empty GameObject that we will use as the XRGrabInteractable's "attachTransform"

        GameObject attach = new GameObject();

        attach.name = gameObject.name + " attachment point";
        attach.transform.parent = transform;
        attach.transform.localPosition = Vector3.zero;
        attach.transform.localRotation = Quaternion.identity;
        attach.transform.localScale = Vector3.one;

        attachTransform = attach.transform;
    }

    // private void Start()
    // {
    //     // Create an empty GameObject that we will use as the XRGrabInteractable's "attachTransform"
    //
    //     // GameObject attach = new GameObject();
    //     //
    //     // attach.name = gameObject.name + " attachment point";
    //     // attach.transform.parent = transform;
    //     // attach.transform.localPosition = Vector3.zero;
    //     // attach.transform.localRotation = Quaternion.identity;
    //     // attach.transform.localScale = Vector3.one;
    //     //
    //     // attachTransform = attach.transform;
    // }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        // This is currently only made for XRRayInteractors, should be easy to support XRDirectInteractors

        if (args.interactor is XRRayInteractor)
        {
            XRRayInteractor rayInteractor = (XRRayInteractor)args.interactor;
            RaycastHit hit;

            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            {
                Vector3 raycastDirection = hit.point - rayInteractor.transform.position;
                // We bump the attachment point 1mm forward (towards the inside of the collider) to avoid some flickering behavior of the RayInteractor
                attachTransform.localPosition = transform.InverseTransformPoint(hit.point + raycastDirection.normalized * 0.001f);
                attachTransform.rotation = rayInteractor.transform.rotation;
            }
        }

        base.OnSelectEntering(args);
    }

    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        if (args.interactor is XRRayInteractor)
        {
            XRRayInteractor rayInteractor = (XRRayInteractor)args.interactor;
            RaycastHit hit;

            if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            {
                Vector3 raycastDirection = hit.point - rayInteractor.transform.position;
                // We bump the attachment point 1mm forward (towards the inside of the collider) to avoid some flickering behavior of the RayInteractor
                attachTransform.localPosition = transform.InverseTransformPoint(hit.point + raycastDirection.normalized * 0.001f);
                attachTransform.rotation = rayInteractor.transform.rotation;
            }
        }
    }
}