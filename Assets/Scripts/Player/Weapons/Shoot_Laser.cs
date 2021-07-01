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

    public override void OnPointDown(Vector3 origin, Vector3 dest, GameObject target)
    {
        
    }

    public override void OnPress(Vector3 origin, Vector3 dest, GameObject target)
    {
        if (curBullet == null && CanShoot())
        {
            _curTime = 0;
            curTarget = target;
            var cell = Instantiate(m_config.prefab, transform);
            curBullet = cell.GetComponent<BulletBase>() as BulletLaser;
            var desc = new MoableDesc()
            {
                _origin = origin,
                _dest = dest,
                _dir = Vector3.Normalize(dest - origin),
                _speed = m_config.speed,
                _life = m_config.life,
                _damgeCallback = OnHit
            };
            curBullet.StartMove(desc);
            m_bullets.Add(curBullet);
        }
        else
        {
            curTarget = target;
            curBullet.Desc._dir = Vector3.Normalize(dest - origin);
            curBullet.Desc._dest = dest;
        }
    }

    public override void OnPointUp(Vector3 origin, Vector3 dest, GameObject target)
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
        if (breakAble != null)
        {
            breakAble.SetSubdivide();
        }

        var muscleCollision = curTarget.gameObject.GetComponent<RootMotion.Dynamics.MuscleCollisionBroadcaster>();
        if (muscleCollision != null)
        {
            muscleCollision.Hit(100, m_config.power * curBullet.Desc._dir, curBullet.Desc._dest);
            Explode(bullect, bullect.Desc._dest);
        }
        else
        {
            Explode(bullect, bullect.Desc._dest);
        }
       
    }
}
