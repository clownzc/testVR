using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹的基类
/// </summary>
public class BulletBase : MonoBehaviour, IMovable
{
    protected MoableDesc _desc;
    public MoableDesc Desc { get { return _desc; } }//参数
    protected float _curLife;
    public float CurLife { get { return _curLife; } }
    public bool HasLife { get { return _curLife > 0; } }
    public virtual void StartMove(MoableDesc desc)
    {
        _desc = desc;
        _curLife = _desc._life;
    }

    public virtual void UpdateMove()
    {
        _curLife -= Time.deltaTime;
        transform.Translate(_desc._dir * _desc._speed);
    }

    public virtual void EndMove()
    {
        Destroy(gameObject);
    }
}
