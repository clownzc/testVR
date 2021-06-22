using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可以移动的物体，先定义接口后面添加轨迹
/// </summary>
public interface IMovable
{
    void StartMove(MoableDesc desc);

    void UpdateMove();

    void EndMove();
}

public class MoableDesc
{
    public Vector3 _origin;
    public Vector3 _dest;
    public Vector3 _dir;
    public float _speed;
    public float _life;
    public Action<BulletBase, Collision> _collisionEnterCallback;//检测碰撞
    public Action<BulletBase> _damgeCallback;//造成伤害
}