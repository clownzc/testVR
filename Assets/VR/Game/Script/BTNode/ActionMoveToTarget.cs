
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
    [TaskDescription("Action: move to target")]
    [TaskCategory("Custom")]
    public class ActionMoveToTarget : Action
    {
        [Tooltip("How far to predict the distance ahead of the target. Lower values indicate less distance should be predicated")]
        public SharedFloat _targetDistPrediction = 20;
        [Tooltip("Multiplier for predicting the look ahead distance")]
        public SharedFloat _targetDistPredictionMult = 20;

        public SharedFloat _moveSpeed;
        public SharedGameObject _target;
        public SharedFloat _attackDistance;
        
        private Vector3 _targetPosition;
        private NavMeshAgent _navMeshAgent;

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

            _targetPosition = _target.Value.transform.position;
            SetDestination(GetTargetPosition());
        }

        public override TaskStatus OnUpdate()
        {
            if (HasArrived()) {
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

        private bool HasArrived()
        {
            float remainingDistance;
            if (_navMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = _navMeshAgent.remainingDistance;
            }

            return remainingDistance <= _attackDistance.Value;
        }
        
        private Vector3 GetTargetPosition()
        {
            var distance = (_target.Value.transform.position - transform.position).magnitude;
            var speed = _navMeshAgent.velocity.magnitude;

            float futurePrediction = 0;
            // Set the future prediction to max prediction if the speed is too small to give an accurate prediction
            if (speed <= distance / _targetDistPrediction.Value) {
                futurePrediction = _targetDistPrediction.Value;
            } else {
                futurePrediction = (distance / speed) * _targetDistPredictionMult.Value; // the prediction should be accurate enough
            }

            // Predict the future by taking the velocity of the target and multiply it by the future prediction
            var prevTargetPosition = _targetPosition;
            _targetPosition = _target.Value.transform.position;
            return _targetPosition + (_targetPosition - prevTargetPosition) * futurePrediction;
        }
    }
}