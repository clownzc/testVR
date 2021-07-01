

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
	[RequireComponent(typeof(Animator))]
	public class CharacterAnimation : MonoBehaviour
	{
		[SerializeField] 
		private CharacterController _characterController;

		private Animator _animator;

		private void Start()
		{
			_animator = GetComponent<Animator>();
		}
		
		private void OnAnimatorMove()
		{
			if (_animator.deltaPosition.Equals(Vector3.zero) && _animator.deltaRotation.Equals(Quaternion.identity))
			{
				return;
			}
			print(_animator.deltaPosition + ", " + _animator.deltaRotation);
			_characterController.Move(_animator.deltaPosition, _animator.deltaRotation);
		}
	}
}
