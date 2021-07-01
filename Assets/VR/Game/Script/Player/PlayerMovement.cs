using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
	public class PlayerMovement : MonoBehaviour
	{
		[SerializeField] private PlayerHand _leftHand;
		[SerializeField] private PlayerHand _rightHand;
		
		private PlayerHand _worldGrabbingHand = null;
		private Vector3 _worldGrabPos;
		
		private void Update()
		{
			RunMovement();
		}

		private Vector3 GetRelativePos(Vector3 pos)
		{
			return pos - transform.position;
		}

		private void TryMove(Vector3 move)
		{
			transform.position -= move;
		}

		private void RunMovement()
		{
			// 判断正在移动输入的手;
			if (_worldGrabbingHand == null)
			{
				if (_leftHand.IsMoveButtonPress())
				{
					_worldGrabbingHand = _leftHand;
				}
				else if (_rightHand.IsMoveButtonPress())
				{
					_worldGrabbingHand = _rightHand;
				}
				if (_worldGrabbingHand != null)
				{
					_worldGrabPos = GetRelativePos(_worldGrabbingHand.transform.position);
				}
			}
			
			// 移动玩家;
			if (_worldGrabbingHand != null)
			{
				if (_worldGrabbingHand.HandType == PlayerHand.Hand.LeftHand
					? _leftHand.IsMoveButtonPress()
					: _rightHand.IsMoveButtonPress())
				{
					var move = GetRelativePos(_worldGrabbingHand.transform.position) - _worldGrabPos;
					move.y = 0f;
					TryMove(move);
					_worldGrabPos = GetRelativePos(_worldGrabbingHand.transform.position);
				}
				else
				{
					_worldGrabbingHand = null;
				}
			}
		}
	}
}
