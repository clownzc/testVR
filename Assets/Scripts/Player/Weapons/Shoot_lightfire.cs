using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot_lightfire : MonoBehaviour, IShoot
{
    private GameObject m_prefab;
    public void Init()
    {
        Debug.Log("Init light fire!");
    }

    public void Switch()
    {
        Debug.Log("Switch light fire!");
    }

    public void Shoot(Vector3 origin, Vector3 dir)
    {
        var cell = Instantiate(m_prefab, origin, Quaternion.identity);
    }
}
