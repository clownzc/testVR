using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物诞生点
/// </summary>
public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] EnemySpawnPointConfig m_config;

    private int m_createNum = 0;//当前创造的数目
    private float lastCreateTime = 0;//上次实例化的时间
    // Update is called once per frame
    void Update()
    {
        if(CanCreate())
        {
            CreateEnemy();
        }
    }

    private bool CanCreate()
    {
        if (m_config.enemyBase == null) return false;
        if (m_config.maxCount < m_createNum) return false;
        if(Time.unscaledTime - lastCreateTime < m_config.createInterval)
        {          
            return false;
        }
        return true;
    }

    private void CreateEnemy()
    {
        lastCreateTime = Time.unscaledTime;
        ++m_createNum;
        Instantiate(m_config.enemyBase, transform.position + new Vector3(UnityEngine.Random.Range(-m_config.offsetRange, m_config.offsetRange)
            , transform.position.y
            , UnityEngine.Random.Range(-m_config.offsetRange, m_config.offsetRange))
            , Quaternion.identity);
    }
}
[Serializable]
public class EnemySpawnPointConfig
{
    public GameObject enemyBase;//敌人实例
    public int maxCount = 10;//敌人的最大实例化数目
    public int offsetRange = 10;//位置偏移范围
    public int createInterval = 10;//创建的时间间隔
}
