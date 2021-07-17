using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动隔段时间加个力
/// </summary>
public class AutoAddForce : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    [SerializeField] float timeInterval = 1;
    [SerializeField] LayerMask shardLayers;
    private float time;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > timeInterval)
        {
            time = 0;
            rigidbody.AddForce(dir);
        }
    }

    //发生碰撞
    private void OnCollisionEnter(Collision collision)
    {
        var breakAble = collision.collider.gameObject.GetComponent<BreakOnTime>();
        bool canBreak = breakAble != null;
         if (canBreak)
        {
            breakAble.SetSubdivide();
        }

        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        Explode(1000, pos);
    }

    public void Explode(float power, Vector3 position)
    {
        foreach (var shard in Physics.OverlapSphere(position, 1, shardLayers))
        {
            var rb = shard.GetComponent<Rigidbody>();
           
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddExplosionForce(power, position, 1);
            }
        }
    }

}
