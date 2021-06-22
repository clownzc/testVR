using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLightFire : BulletBase
{
    public override void StartMove(MoableDesc desc)
    {
        base.StartMove(desc);
    }

    public override void UpdateMove()
    {
        base.UpdateMove();
    }

    public override void EndMove()
    {
        base.EndMove();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit!");
        _desc._collisionEnterCallback(this, collision);
    }
}
