using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeWeapon : MonoBehaviour
{
    [SerializeField] float limitMagnitude = 2;
    [SerializeField] float powerScale = 100;//根据速度
    [SerializeField] LayerMask shardLayers;
    [SerializeField] AudioSource autio;

    //发生碰撞
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < limitMagnitude) return;
        var breakAble = collision.collider.gameObject.GetComponent<BreakOnTime>();
        bool canBreak = breakAble != null;
        if (canBreak)
        {
            breakAble.SetSubdivide();
        }
        
       if (autio) autio.Play();
        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        WeaponUtil.DestructBuild(collision.collider.gameObject, pos);
        Explode(collision.relativeVelocity.magnitude * powerScale, pos);
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
