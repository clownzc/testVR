using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;

public abstract class ShootBase : MonoBehaviour
{
    [SerializeField] protected ShootConfig m_config;
   
    protected float _curTime;

    protected List<BulletBase> m_bullets = new List<BulletBase>();
    public virtual void Init() { }

    /// <summary>
    /// 放下这把武器
    /// </summary>
    public virtual void DeactiveWeapon() { }

    /// <summary>
    /// 选择这把武器
    /// </summary>
    public virtual void ActiveWeapon() { }

    /// <summary>
    /// 射击
    /// </summary>
    public virtual void OnPointDown() 
    {
    }

    public virtual void OnPointUp()
    {
    }

    public virtual void OnPress()
    {
    }

    public virtual bool CanShoot() 
    {
        return _curTime > m_config.interval;
    }

    public virtual void Explode(BulletBase bullect, Vector3 position)
    {
        foreach (var shard in Physics.OverlapSphere(position, m_config.explosionRadius, m_config.shardLayers))
        {
            var rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(m_config.power, position, m_config.explosionRadius);              
               // shard.gameObject.AddComponent<AutoDestruct>().Time = 10.0f;
            }
        }
    }

    private void Update()
    {
        _curTime += Time.deltaTime;
        for (int i = 0; i < m_bullets.Count; ++i)
        {
            m_bullets[i].UpdateMove();
            if(m_bullets[i].HasLife == false)
            {
                DeleteBullet(m_bullets[i]);
            }
        }
    }

    public void DeleteBullet(BulletBase bullet)
    {
        m_bullets.Remove(bullet);
        bullet.EndMove();
    } 

    public int GetHitLayer()
    {
        int layA = LayerMask.NameToLayer("BreakAble");
        int layB = LayerMask.NameToLayer("Enemy");
        int layC = LayerMask.NameToLayer("Default");
        return 1 << layA | 1 << layB | 1 << layC;
    }
}
