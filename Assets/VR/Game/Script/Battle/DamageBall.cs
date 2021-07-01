
using UnityEngine;

namespace Battle
{
    [RequireComponent(typeof(Rigidbody))]
    public class DamageBall : MonoBehaviour
    {
        private Rigidbody _rbody;
    
        private void Start()
        {
            _rbody = GetComponent<Rigidbody>();
        }
    
        private void OnCollisionEnter(Collision other)
        {
            var choppable = other.gameObject.GetComponentInParent<Choppable>();
            if (choppable == null) return;
        
            print("choppable!");
            var enemy = choppable.GetComponentInParent<Enemy>();
            if (enemy == null) return;
        
            print("chop!!!!");
            enemy.Chop(choppable, other, gameObject);
        } 
    }
}