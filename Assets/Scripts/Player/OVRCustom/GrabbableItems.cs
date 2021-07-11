using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以抓的物体
/// </summary>
public class GrabbableItems : OVRGrabbable
{
    override public void GrabBegin(OVRGrabber hand, Collider grabPoint)
    {
        Debug.Log($"拿起物体：{grabPoint.gameObject}");
        base.GrabBegin(hand, grabPoint);
    }

    /// <summary>
    /// Notifies the object that it has been released.
    /// </summary>
    virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        Debug.Log("放下物体");
        base.GrabEnd(linearVelocity, angularVelocity);
    }
}
