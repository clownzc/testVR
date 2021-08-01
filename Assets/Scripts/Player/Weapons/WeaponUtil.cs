using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUtil
{
    //建筑摧毁
    public static void DestructBuild(GameObject shard, Vector3 position)
    {
        //激活frac刚体层
        int layer = LayerMask.NameToLayer("BreakAble");
        for (int i = 0; i < shard.transform.childCount; ++i)
        {
            var obj = shard.transform.GetChild(i);
            if (obj.name == "frac")
            {
                obj.SetParent(null);
                obj.gameObject.SetActive(true);
                foreach (Transform tran in obj.GetComponentsInChildren<Transform>())
                {
                    tran.gameObject.layer = layer;//更改物体的Layer层
                }
            }
            else
            {
                var child = obj.FindChildRecursive("frac");
                if (child)
                {
                    child.SetParent(null);
                    child.gameObject.SetActive(true);
                    foreach (Transform tran in child.GetComponentsInChildren<Transform>())
                    {
                        tran.gameObject.layer = layer;//更改物体的Layer层
                    }
                }
            }
        }
        shard.gameObject.SetActive(false);
    }
}
