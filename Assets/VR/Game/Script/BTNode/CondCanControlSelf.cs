

using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BTNode
{
	[TaskDescription("Condition: can control self?")]
	[TaskCategory("Custom")]
	public class CondCanControlSelf : Conditional
	{
		public SharedGameObject _animatorGameObject;

		private Animator _animator;

		public override void OnStart()
		{
            _animator = _animatorGameObject.Value.GetComponent<Animator>();
		}
		
		public override TaskStatus OnUpdate()
		{
			var state = _animator.GetCurrentAnimatorStateInfo(0);
			if (state.IsName("Fall") || 
			    state.IsName("GetUpProne") || 
			    state.IsName("GetUpSupine") || 
			    state.IsName("Dizzy"))
			{
				return TaskStatus.Failure;
			}
			else
			{
				return TaskStatus.Success;
			}
		}
	}
}
