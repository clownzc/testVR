
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
	public class Enemy : MonoBehaviour
	{
		public SkinnedMeshRenderer skinnedMeshRenderer;
		public Material fillMaterial;
		public PuppetMaster puppetMaster;
		public EnemyPuppet enemyPuppet;

		public bool dead = false;

		public class RemoveMuscleInfo
		{
			public ConfigurableJoint joint;
			public GameObject choppedGameObject;
		}

		private List<RemoveMuscleInfo> _removeMuscleJoints;

		private bool _choppable = true;
		private float _chopCooldown = 1f;
		private float _chopTimer = 0f;

		private void Awake()
		{
			_removeMuscleJoints = new List<RemoveMuscleInfo>();
		}

		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1);
			puppetMaster.gameObject.SetActive(true);
			var list = puppetMaster.GetComponentsInChildren<Collider>();
			foreach (var collider in list) {
				collider.enabled = true;
			}
			var bt = gameObject.GetComponentInChildren<BehaviorDesigner.Runtime.BehaviorTree>();
			bt.EnableBehavior();
		}

		private void Update()
		{
			if (_removeMuscleJoints.Count > 0)
			{
				foreach (var info in _removeMuscleJoints)
				{
					puppetMaster.RemoveMuscleRecursive(info.joint, true, true);
					info.joint.transform.parent = info.choppedGameObject.transform;
					info.joint.GetComponent<Rigidbody>().drag = 0.1f;
				}
				_removeMuscleJoints.Clear();
			}

			if (!_choppable)
			{
				_chopTimer += Time.deltaTime;
				if (_chopTimer > _chopCooldown)
				{
					_choppable = true;
				}
			}
		}

		public bool Chop(Choppable choppable, Collision collision, GameObject byObject)
		{
			if (!_choppable) return false;

			Vector3 hitPoint = Vector3.zero;
			Vector3 v = Vector3.zero;
			if (choppable.chopPoint != null)
			{
				hitPoint = choppable.chopPoint.position;
				//v = choppable.chopPoint.forward;
				v = collision.relativeVelocity;
				print("chop normal: " + v);
			}
			else
			{
				hitPoint = collision.contacts[0].point;
				v = collision.relativeVelocity;
			}
			var result = ChopScript.Chop(choppable, fillMaterial, hitPoint, v, gameObject);

			_choppable = false;
			_chopTimer = 0f;

			return result;
		}

		public void RemoveMuscle(Choppable choppable, GameObject choppedGameObject)
		{
			var info = new RemoveMuscleInfo()
			{
				joint = choppable.GetComponent<ConfigurableJoint>(),
				choppedGameObject = choppedGameObject,
			};
			_removeMuscleJoints.Add(info);
		}

		public void Die()
		{
			if (dead) return;
			dead = true;
			
			puppetMaster.state = PuppetMaster.State.Dead;
			var bt = GetComponentInChildren<BehaviorDesigner.Runtime.BehaviorTree>();
			if (bt != null)
			{
				bt.enabled = false;
			}
		}

		public void FallWithoutGetUp()
		{
			enemyPuppet.LoseBalance();
			enemyPuppet.canGetUp = false;
		}
	}
}
