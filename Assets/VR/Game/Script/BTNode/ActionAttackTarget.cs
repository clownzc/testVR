
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

namespace BTNode
{
    [TaskDescription("Action: attack target")]
    [TaskCategory("Custom")]
    public class ActionAttackTarget : Action
    {
        public SharedGameObject _target;
        public SharedGameObject _animatorGameObject;
        //public SharedFloat _attackInterval;

        private float _timer;
        private Animator _animator;

        public override void OnStart()
        {
            _timer = 0f;
            _animator = _animatorGameObject.Value.GetComponent<Animator>();
        }
        
        public override TaskStatus OnUpdate()
        {
            var dir = _target.Value.transform.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            var state = _animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("Attack") && state.normalizedTime >= 1f) 
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
    }
}