

using System.Collections;
using System.Collections.Generic;
using Player;
using RootMotion.Dynamics;
using UnityEngine;

namespace Battle
{
	public class EnemyDamage : MuscleCollisionHandlerBase
	{
		[SerializeField] private GameObject _damageTextPrefab;
		[SerializeField] private float _immunityTime;
		[SerializeField] private float _damageImpulse;
		[SerializeField] private float _loseBalanceImpulse;
		[SerializeField] private float _maxHp = 1000f;
		[SerializeField] private float _hp;

		public EnemyPuppet Puppet { get; set; }

		private Enemy _enemy;
		private bool _immunity;
		private float _immunityTimer;

		public override void OnMuscleCollisionHandlerInitialized(EnemyPuppet puppet)
		{
			Puppet = puppet;
		}

		public override void OnMuscleHitBehaviour(MuscleHit hit)
		{
			print("muscle hit!");
			TakeDamage(hit.position, hit.unPin, null, null);
		}

		public override void OnMuscleCollisionBehaviour(MuscleCollision m)
		{
			if (m.isStay) return;

			var impulse = m.collision.impulse.magnitude;
			if (impulse <= _damageImpulse) return;

			var muscle = Puppet.puppetMaster.muscles[m.muscleIndex];
			
			TakeDamage(m.collision.contacts[0].point, impulse, m.collision, muscle);
		}

		private void Start()
		{
			_enemy = GetComponentInParent<Enemy>();
			_hp = _maxHp;
		}

		private void Update()
		{
			var dt = Time.deltaTime;

			if (_immunity)
			{
				_immunityTimer -= dt;
				if (_immunityTimer <= 0f) _immunity = false;
			}
		}

		private void SetImmunity()
		{
			_immunity = true;
			_immunityTimer = _immunityTime;
		}

		private void HandShock(Collision collision, float damage, bool chopped)
		{
			var weapon = collision.gameObject.GetComponentInParent<Weapon>();
			if (weapon != null && weapon.hand != null)
			{
				Messenger.Broadcast<PlayerHand, float, bool>("HandShock",
					weapon.hand, damage, chopped);
				print("hand shock: " + weapon.hand);
			}
		}

		private void TakeDamage(Vector3 pos, float damage, Collision collision, Muscle muscle)
		{
			// 击中身体音效;
			Messenger.Broadcast("PlaySoundRandom", AudioList.BodyHit, pos);

			
			if (_immunity) return;
			print("take damage 2: " + damage);

			if (!_enemy.dead)
			{
				_hp -= damage;
				if (_hp <= 0)
				{
					_enemy.Die();
					return;
				}
			}

			if (!_enemy.dead && _damageTextPrefab != null)
			{
				var go = Instantiate(_damageTextPrefab, pos, Quaternion.identity);
				var textMesh = go.GetComponent<TextMesh>();
				if (textMesh != null)
				{
					textMesh.text = Mathf.FloorToInt(damage).ToString();
				}
			}

			SetImmunity();

			print("damage: " + damage + ", " + ", " + _loseBalanceImpulse);
			if (collision == null || muscle == null) return;//先不处理激光的其他效果

			var chopped = false;
			var choppable = muscle.joint.GetComponent<Choppable>();
			if (choppable != null)
			{
				if (damage > choppable.chopThreshold)
				{
					var weapon = collision.gameObject.GetComponentInParent<Weapon>();
					if (weapon != null && weapon.chop)
					{
						var result = _enemy.Chop(choppable, collision, collision.gameObject);
						if (result)
						{
							chopped = true;
							
							// 断肢音效;
							Messenger.Broadcast("PlaySoundRandom", AudioList.ArmorCrash, pos);

							var die = false;
							var group = muscle.props.group;
							if (group == Muscle.Group.Head || group == Muscle.Group.Hips ||
							    group == Muscle.Group.Spine)
							{
								die = true;
								_enemy.Die();
            					Messenger.Broadcast("CrowdAction", 3);
							}
							else if (group == Muscle.Group.Leg || group == Muscle.Group.Foot)
							{
								_enemy.FallWithoutGetUp();
							}

							if (!die)
							{
	            				Messenger.Broadcast("CrowdAction", 2);
							}
						}
					}
				}
			}
			HandShock(collision, damage, chopped);
			
			if (!_enemy.dead && damage > _loseBalanceImpulse)
			{
				Puppet.LoseBalance();
			}
		}
	}
}
