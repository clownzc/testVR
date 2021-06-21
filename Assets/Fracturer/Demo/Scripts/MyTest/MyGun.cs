using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;
/// <summary>
/// 用来测试实时生成网格的运行效率
/// </summary>
public class MyGun : MonoBehaviour
{
    public LayerMask ShardLayers;
    public float ExplosionRadius;
    public float ExplosionForce;

    [SerializeField]
    private Camera myCamera;
    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            var ray = new Ray(myCamera.transform.position, myCamera.transform.forward);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log($"HIT:{hit.collider.gameObject}");
                var breakAble = hit.collider.gameObject.GetComponent<BreakOnTime>();
                if(breakAble != null)
                {
                    breakAble.SetSubdivide();
                }
                Explode(hit.point);
            }
        }
    }

    void Explode(Vector3 position)
    {
        foreach (var shard in Physics.OverlapSphere(position, ExplosionRadius, ShardLayers))
        {
            var rb = shard.GetComponent<Rigidbody>();
            if(rb!= null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(ExplosionForce, position, ExplosionRadius);
                shard.gameObject.AddComponent<AutoDestruct>().Time = 10.0f;
            }        
        }
    }
}
