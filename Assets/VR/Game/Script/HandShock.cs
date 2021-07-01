using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class HandShock : MonoBehaviour
{
	private PlayerHand _hand;
	private float _damage;
	private bool _chopped;
	
	public float damageMul = 10f;
	public float choppedDamageMul = 10f;
	public float minDamage = 1000f;
	public float maxDamage = 300000f;
	public float duration = 0.1f;
	public float choppedDuration = 0.3f;
	
	private void OnEnable()
	{
		Messenger.AddListener<PlayerHand, float, bool>("HandShock", OnHandShock);
	}
	
	private void OnDisable()
	{
		Messenger.RemoveListener<PlayerHand, float, bool>("HandShock", OnHandShock);
	}

	private void OnHandShock(PlayerHand hand, float damage, bool chopped)
	{
		StopCoroutine("Shock");
		_hand = hand;
		_damage = damage;
		_chopped = chopped;
		StartCoroutine("Shock");
	}
	
	private IEnumerator Shock()
	{
		var hand = _hand;
		var damage = _damage;
		var chopped = _chopped;

		damage = _chopped ? (damage * choppedDamageMul) : (damage * damageMul);
		damage = Mathf.Clamp(damage, minDamage, maxDamage);
		
		var time = _chopped ? choppedDuration : duration;

		//Debug.LogError("damage: " + damage);
		
		float shockTimer = 0f;
		while (shockTimer <= time)
		{
			hand.HandShock((ushort)damage);
			shockTimer += Time.deltaTime;
			yield return null;
		}
	}
}
