

using System.Collections;
using System.Collections.Generic;
using RootMotion;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
	public class EnemyPuppet : BehaviourPuppet
	{
		[LargeHeader("Extensions")]
		[SerializeField] 
		private MuscleCollisionHandlerBase _muscleCollisionHandler;

		private void Start()
		{
			if (_muscleCollisionHandler != null)
			{
				_muscleCollisionHandler.OnMuscleCollisionHandlerInitialized(this);
			}
		}
		
		protected override void OnMuscleHitBehaviour(MuscleHit hit) 
		{
			if (!enabled) return;
			
			if (_muscleCollisionHandler != null)
			{
				_muscleCollisionHandler.OnMuscleHitBehaviour(hit);
			}
		}

		protected override void OnMuscleCollisionBehaviour(MuscleCollision m) 
		{
			if (!enabled) return;

			if (_muscleCollisionHandler != null)
			{
				_muscleCollisionHandler.OnMuscleCollisionBehaviour(m);
			}
		}

		public void LoseBalance()
		{
			print("lose balance!");
			SetState(State.Unpinned);
		}
	}
}
