using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器控制器，负责武器的操作
/// </summary>
public class WeaponController : MonoBehaviour
{
    //可以选择的武器
    private List<IShoot> m_shoots = new List<IShoot>();

    private int m_curSelectIndex;//当前选择的武器编号

    private void Init()
    {
        //初始化可以使用的服务器
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchShooter();
        }
    }

    /// <summary>
    /// 选择射击器
    /// </summary>
    private void SwitchShooter()
    {
        ++m_curSelectIndex;
        if (m_curSelectIndex == m_shoots.Count)
        {
            m_curSelectIndex = 0;
        }
        m_shoots[m_curSelectIndex].Switch();
    }

    public void Shoot(Vector3 origin, Vector3 dir)
    {
        m_shoots[m_curSelectIndex].Shoot(origin, dir);
    }
}
