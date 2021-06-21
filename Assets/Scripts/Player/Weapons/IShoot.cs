using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShoot 
{
    void Init();

    /// <summary>
    /// 选择这把武器
    /// </summary>
    void Switch();

    /// <summary>
    /// 射击
    /// </summary>
    void Shoot(Vector3 origin, Vector3 dir);
}
