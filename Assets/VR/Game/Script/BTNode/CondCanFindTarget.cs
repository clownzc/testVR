

using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BTNode
{
    [TaskDescription("Condition: can find target?")]
    [TaskCategory("Custom")]
	public class CondCanFindTarget : Conditional
	{
		public SharedString _targetTag;
		public SharedGameObject _target;

		public override TaskStatus OnUpdate()
		{
			var tag = _targetTag.Value;
			if (string.IsNullOrEmpty(tag))
			{
				return TaskStatus.Failure;
			}
			
			var go = GameObject.FindGameObjectWithTag(tag);
			if (go != null)
			{
				_target.SetValue(go);
				return TaskStatus.Success;
			}
			else
			{
				return TaskStatus.Failure;
			}
		}
	}
}
