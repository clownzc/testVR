using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;
//投石器
public class Shoot_rock : ShootBase
{
    public override void Init()
    {
        Debug.Log("Init rock!");
    }

    public override void DeactiveWeapon()
    {
        Debug.Log("DeactiveWeapon rock!");
    }

    public override void ActiveWeapon()
    {
        Debug.Log("ActiveWeapon rock!");
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
            _dir = Vector3.Normalize(dest + Vector3.up * 5 - origin),
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
       // DeleteBullet(bullect);
    }

    public override void Explode(BulletBase bullect, Vector3 position)
    {
        foreach (var shard in Physics.OverlapSphere(position, m_config.explosionRadius, m_config.shardLayers))
        {
            var rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                var power = bullect.HasLife ? m_config.power - bullect.CurLife * m_config.powerDecayByTime : 0;
                rb.AddExplosionForce(power, position, m_config.explosionRadius);
                shard.gameObject.AddComponent<AutoDestruct>().Time = 10.0f;
            }
        }
    }
}
