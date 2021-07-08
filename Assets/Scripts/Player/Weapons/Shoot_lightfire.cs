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

    public override void OnPointUp ()
    {
        RaycastHit hit;
        var sightObj = m_config.sight != null ? m_config.sight : transform;
        var ray = new Ray(sightObj.position, sightObj.forward);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (CanShoot() == false) return;
            _curTime = 0;
            var cell = Instantiate(m_config.prefab, sightObj.position, Quaternion.identity);
            var bullect = cell.GetComponent<BulletBase>();
            var desc = new MoableDesc()
            {
                _origin = sightObj.position,
                _dest = hit.point,
                _dir = Vector3.Normalize(hit.point - sightObj.position),
                _speed = m_config.speed,
                _life = m_config.life,
                _collisionEnterCallback = OnHit
            };
            bullect.StartMove(desc);
            m_bullets.Add(bullect);
        }
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
