
// 参考Pursue.cs

using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using Tooltip = BehaviorDesigner.Runtime.Tasks.TooltipAttribute;

namespace BTNode
{
    [TaskDescription("Action: keep away from target")]
    [TaskCategory("Custom")]
    public class ActionKeepAwayFromTarget : Action
    {
        public SharedFloat _moveSpeed;
        public SharedFloat _keepAwayTime = 1f;
        public SharedGameObject _target;
        
        private NavMeshAgent _navMeshAgent;
        private float _timer;

        public override void OnAwake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
    
        public override void OnStart()
        {
            _navMeshAgent.speed = _moveSpeed.Value;
            _navMeshAgent.angularSpeed = 120;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.updateRotation = false;

            _timer = 0f;

            SetDestination(GetTargetPosition());
        }

        public override TaskStatus OnUpdate()
        {
            var dt = Time.deltaTime;
            _timer += dt;
            if (_timer > _keepAwayTime.Value)
            {
                return TaskStatus.Success;
            }

            var dir = _target.Value.transform.position - transform.position;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            SetDestination(GetTargetPosition());
            return TaskStatus.Running;
        }
        
        public override void OnEnd()
        {
            Stop();
        }

        public override void OnBehaviorComplete()
        {
            Stop();
        }

        private void Stop()
        {
            if (_navMeshAgent.hasPath)
            {
                _navMeshAgent.ResetPath();
                _navMeshAgent.isStopped = true;
            }
        }

        private void SetDestination(Vector3 destination)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
        }

        private Vector3 GetTargetPosition()
        {
            var v = (transform.position - _target.Value.transform.position).normalized;
            v *= _keepAwayTime.Value * _navMeshAgent.speed;
            var targetPosition = transform.position + v;
            return targetPosition;
        }
    }
}