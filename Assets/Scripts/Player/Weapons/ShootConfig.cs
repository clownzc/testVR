using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ShootConfig 
{
    public GameObject prefab;
    public float speed;
    public float interval;
  
    public LayerMask shardLayers;
    public float explosionRadius;
    public float life;

    public float power;
    public float powerDecayByTime;//战斗力随时间下降
}
