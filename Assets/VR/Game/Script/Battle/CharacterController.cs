
using System.Collections;
using System.Collections.Generic;
using RootMotion;
using UnityEngine;

namespace Battle
{
	public class CharacterController : MonoBehaviour
	{
		private Vector3 _fixedDeltaPosition;
		private Quaternion _fixedDeltaRotation;

		private Rigidbody _rbody;

		private void Start()
		{
			_rbody = GetComponent<Rigidbody>();
		}

		private void MoveFixed(Vector3 deltaPosition)
		{
			var gravity = Physics.gravity;
			var velocity = deltaPosition / Time.deltaTime;
			Vector3 verticalVelocity = V3Tools.ExtractVertical(_rbody.velocity, gravity, 1f);
			Vector3 horizontalVelocity = V3Tools.ExtractHorizontal(velocity, gravity, 1f);
			//print(horizontalVelocity + ", " + verticalVelocity);
			_rbody.velocity = horizontalVelocity + verticalVelocity;
		}
		
		private void FixedUpdate()
		{
			MoveFixed(_fixedDeltaPosition);
			_fixedDeltaPosition = Vector3.zero;

			transform.rotation *= _fixedDeltaRotation;
			_fixedDeltaRotation = Quaternion.identity;
		}
		
		public void Move(Vector3 deltaPosition, Quaternion deltaRotation) 
		{
			_fixedDeltaPosition += deltaPosition;
			_fixedDeltaRotation *= deltaRotation;
		}
	}
}
