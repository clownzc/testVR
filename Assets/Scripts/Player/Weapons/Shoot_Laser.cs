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

    public override void Shoot(Vector3 origin, Vector3 dest, GameObject target)
    {
        curTarget = target;
        if(curBullet == null)
        {
            base.Shoot(origin, dest, target);
            var cell = Instantiate(m_config.prefab, origin, Quaternion.identity, transform);
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
            curBullet.Desc._dir = Vector3.Normalize(dest - origin);
            curBullet.Desc._dest = dest;
        }
    }

    private void OnHit(BulletBase bullect)
    {
        Debug.Log($"Hit target :{curTarget}");
        if (curTarget == null) return;
        var breakAble = curTarget.GetComponent<BreakOnTime>();
        if (breakAble != null)
        {
            breakAble.SetSubdivide();
        }
        Explode(bullect, bullect.Desc._dest);
    }
}
