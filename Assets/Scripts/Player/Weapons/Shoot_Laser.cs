using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 发射闪电
/// </summary>
public class Shoot_Laser : ShootBase
{
    private BulletLaser curBullet;
    private GameObject curTarget;
  
    public override void Init()
    {
        Debug.Log("Init Laser!");
    }

    public override void DeactiveWeapon()
    {
        Debug.Log("DeactiveWeapon Laser!");
    }

    public override void ActiveWeapon()
    {
        Debug.Log("ActiveWeapon Laser!");
    }

    public override bool CanShoot()
    {
        return curBullet == null;
    }

    public override void OnPointDown()
    {
      
    }

    public override void OnPress()
    {
        RaycastHit hit;
        var sightObj = m_config.sight != null ? m_config.sight : transform;
        var ray = new Ray(sightObj.position, sightObj.forward);
       
        if (Physics.Raycast(ray, out hit, 100.0f, GetHitLayer()))
        {
            if (curBullet == null && CanShoot())
            {
                _curTime = 0;
                curTarget = hit.collider.gameObject;
                var cell = Instantiate(m_config.prefab, sightObj.position, Quaternion.identity);
                curBullet = cell.GetComponent<BulletBase>() as BulletLaser;
                var desc = new MoableDesc()
                {
                    _origin = sightObj.position,
                    _dest = hit.point,
                    _dir = Vector3.Normalize(hit.point - sightObj.position),
                    _speed = m_config.speed,
                    _life = m_config.life,
                    _damgeCallback = OnHit
                };
                curBullet.StartMove(desc);
                m_bullets.Add(curBullet);
            }
            else
            {
                curTarget = hit.collider.gameObject;
                curBullet.Desc._dir = Vector3.Normalize(hit.point - sightObj.position);
                curBullet.Desc._dest = hit.point;
                curBullet.Desc._origin = sightObj.position;
            }
        }      
    }

    public override void OnPointUp()
    {
        Debug.Log("ShootLaser OnPointUp!");
        if (curBullet != null)
        {
            DeleteBullet(curBullet);
        }
    }

    private void OnHit(BulletBase bullect)
    {
        if (curTarget == null) return;
        var breakAble = curTarget.GetComponent<BreakOnTime>();
        bool canBreak = breakAble != null;
        if (canBreak)
        {
            breakAble.SetSubdivide();
        }

        var muscleCollision = curTarget.gameObject.GetComponent<RootMotion.Dynamics.MuscleCollisionBroadcaster>();
        if (muscleCollision != null)
        {
            muscleCollision.Hit(100, m_config.power * curBullet.Desc._dir, curBullet.Desc._dest);
            Explode(bullect, bullect.Desc._dest, false);
        }
        else
        {
            Explode(bullect, bullect.Desc._dest, canBreak);
        }
       
    }
}
