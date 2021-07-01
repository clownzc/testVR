using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器控制器，负责武器的操作
/// </summary>
public class WeaponController : MonoBehaviour
{
    //可以选择的武器
    [SerializeField]
    private List<ShootBase> m_shoots;

    private int m_curSelectIndex;//当前选择的武器编号

    public void Init()
    {
        //初始化可以使用的服务器
        for(int i = 0; i < m_shoots.Count; ++i)
        {
            m_shoots[i].Init();
        }
    }


    /// <summary>
    /// 选择射击器
    /// </summary>
    public void SwitchShooter()
    {
        m_shoots[m_curSelectIndex].DeactiveWeapon();
        ++m_curSelectIndex;
        if (m_curSelectIndex == m_shoots.Count)
        {
            m_curSelectIndex = 0;
        }
        m_shoots[m_curSelectIndex].ActiveWeapon();
    }

    public void OnPointDown(Vector3 origin, Vector3 dest, GameObject target)
    {
        m_shoots[m_curSelectIndex].OnPointDown(origin, dest, target);
    }

    public void OnPress(Vector3 origin, Vector3 dest, GameObject target)
    {
        m_shoots[m_curSelectIndex].OnPress(origin, dest, target);
    }

    public void OnPointUp(Vector3 origin, Vector3 dest, GameObject target)
    {
        m_shoots[m_curSelectIndex].OnPointUp(origin, dest, target);
    }
}
