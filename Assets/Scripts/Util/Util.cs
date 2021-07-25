using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一些通用方法
/// </summary>
public static class LayerUtil
{
    /// <summary>
    /// 是否属于这个层
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = 1 << obj.layer;
        return (layerMask.value & objLayerMask) > 0;
    }
}
