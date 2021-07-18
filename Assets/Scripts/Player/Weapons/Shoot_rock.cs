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

    public override void OnPointUp()
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
        var breakAble = collision.collider.gameObject.GetComponent<BreakOnTime>();
        bool canBreak = breakAble != null;
        Debug.LogError($"breakAble:{breakAble} canBreak:{canBreak}");
        if (canBreak)
        {
            breakAble.SetSubdivide();
        }

        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        Explode(bullect, pos, false);
       // DeleteBullet(bullect);
    }

    public override void Explode(BulletBase bullect, Vector3 position, bool autoDelete)
    {
        foreach (var shard in Physics.OverlapSphere(position, m_config.explosionRadius, m_config.shardLayers))
        {
            var rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                var power = bullect.HasLife ? m_config.power - bullect.CurLife * m_config.powerDecayByTime : 0;
                rb.AddExplosionForce(power, position, m_config.explosionRadius);
                if(autoDelete) shard.gameObject.AddComponent<AutoDestruct>().Time = 3.0f;
            }
        }
    }
}
