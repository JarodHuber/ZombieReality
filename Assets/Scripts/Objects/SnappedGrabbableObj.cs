using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappedGrabbableObj : GrabbableObj
{
    [SerializeField] Transform snapOffset = null;

    protected override void Initialize()
    {
        base.Initialize();

        if(!snapOffset)
        snapOffset = transform.GetChild(0);
    }

    public override void Grab(Hand handGrabbing)
    {
        transform.rotation = (handGrabbing.transform.rotation * Quaternion.Inverse(snapOffset.rotation)) * transform.rotation;
        transform.position += handGrabbing.transform.position - snapOffset.position;

        base.Grab(handGrabbing);
    }
    public override void SwapHands(Hand newHand)
    {
        transform.rotation = (newHand.transform.rotation * Quaternion.Inverse(snapOffset.rotation)) * transform.rotation;
        transform.position += newHand.transform.position - snapOffset.position;

        base.SwapHands(newHand);
    }
}
