using System.Collections;
using System.Collections.Generic;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
	public abstract class MuscleCollisionHandlerBase : MonoBehaviour
	{
		public abstract void OnMuscleCollisionHandlerInitialized(EnemyPuppet puppet);
		public abstract void OnMuscleHitBehaviour(MuscleHit hit);
		public abstract void OnMuscleCollisionBehaviour(MuscleCollision m);
	}
}

