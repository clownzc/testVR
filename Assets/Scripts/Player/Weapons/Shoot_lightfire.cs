using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_lightfire : ShootBase
{
    
    public override void Init()
    {
        Debug.Log("Init light fire!");
    }

    public override void DeactiveWeapon()
    {
        Debug.Log("DeactiveWeapon light fire!");
    }

    public override void ActiveWeapon()
    {
        Debug.Log("ActiveWeapon light fire!");
    }

    public override void Shoot(Vector3 origin, Vector3 dest, GameObject target)
    {
        base.Shoot(origin, dest, target);
        var cell = Instantiate(m_config.prefab, origin, Quaternion.identity);
        var bullect = cell.GetComponent<BulletBase>();
        var desc = new MoableDesc()
        {
            _origin = origin,
            _dest = dest,
            _dir = Vector3.Normalize(dest - origin),
            _speed = m_config.speed,
            _life = m_config.life,
            _collisionEnterCallback = OnHit
        };
        bullect.StartMove(desc);
        m_bullets.Add(bullect);
    }

    private void OnHit(BulletBase bullect, Collision collision)
    {
        Debug.Log($"Hit target :{collision.gameObject}");
        var breakAble = collision.collider.gameObject.GetComponent<BreakOnTime>();
        if (breakAble != null)
        {
            breakAble.SetSubdivide();
        }

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        Explode(bullect, pos);
    }
}
