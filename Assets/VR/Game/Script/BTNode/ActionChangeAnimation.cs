
using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace BTNode
{
    [TaskDescription("Action: change animation")]
    [TaskCategory("Custom")]
    public class ActionChangeAnimation : Action
    {
        [System.Serializable]
        public class BlendParameter
        {
            public string Key;
            public float Value;
        }
        
        public SharedGameObject _targetGameObject;
        public string _changeToAnimation;
        public bool _blend;
        public SharedFloat _blendTime;
        public BlendParameter[] _blendParameters;

        private Animator _animator;
        private float _currentBlendTime;

        public override void OnStart()
        {
            _animator = _targetGameObject.Value.GetComponent<Animator>();
            if (!string.IsNullOrEmpty(_changeToAnimation))
            {
                var state = _animator.GetCurrentAnimatorStateInfo(0);
                if (!state.IsName(_changeToAnimation))
                {
                    _animator.SetTrigger(_changeToAnimation);
                }
            }
            
            _currentBlendTime = 0f;
            if (!_blend)
            {
                foreach (var p in _blendParameters)
                {
                    _animator.SetFloat(p.Key, p.Value);
                }
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (!_blend || _blendParameters.Length == 0)
            {
                return TaskStatus.Success;
            }
            
            var dt = Time.deltaTime;
            _currentBlendTime += dt;
            var blendTime = _blendTime.Value;
            if (_currentBlendTime > blendTime)
            {
                dt -= (_currentBlendTime - blendTime);
            }

            foreach (var p in _blendParameters)
            {
                var key = p.Key;
                var target = p.Value;
                var value = _animator.GetFloat(key);
                var t = dt / (blendTime - _currentBlendTime);
                _animator.SetFloat(key, Mathf.Lerp(value, target, t));
            }

            if (_currentBlendTime > blendTime)
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