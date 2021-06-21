using System;
using System.Collections;
using System.Collections.Generic;
using ScoredProductions.Global;
using UnityEngine;

namespace ScoredProductions.PGDC
{
	public class ParentLink
	{
		public BlockStats Link;
		public Action action;

		public ParentLink(BlockStats link, Action a) {
			Link = link;
			action = a;
		}
	}

	public class BlockStats : MonoBehaviour, DamageInterface {

		public bool Destructible; // if it will fall apart when damaged

		[Range(0, Mathf.Infinity)]
		public float Health = 1; // Health of the object : need to make it more accessable to the gen structure

		[Range(0, Mathf.Infinity)]
		public float DestroyTime = 10; // How much time passes before the object despawns

		public bool DestroyOnSleep; // If the object is destroyed on sleep

		[NonSerialized]
		public float DamageReceived = 0; // Current damage to be processed
		public void ReceiveDamage(float damage) { DamageReceived += damage; }

		[NonSerialized]
		public bool Destroyed = false; // If the objects death has been triggered

		public List<ParentLink> DestroyTriggered = new List<ParentLink>();

		public bool TriggerChildren;
		private bool loadedchildren;
		public List<BlockStats> AllChildStatsCode = new List<BlockStats>();

		[NonSerialized]
		public Vector3 PositionInGroup = new Vector3();

		// Update is called once per frame
		void Update() {
			if (Input.GetKey(KeyCode.F) && Destructible) { // To test the destruction
				DamageReceived = Health + 1;
			}

			ProcessDamage();

			if (Health <= 0 && !Destroyed) {
				DestroySequence();
				TriggerActions();
			}
			if (!loadedchildren) {
				loadedchildren = true;
				if (AllChildStatsCode == null || AllChildStatsCode.Count == 0) {
					AllChildStatsCode = new List<BlockStats>();
					AllChildStatsCode.AddRange(GetComponentsInChildren<BlockStats>());
				}
			}
		}

		public void TriggerActions() {
			DestroyTriggered.ForEach(e => e.action.Invoke());
		}

		public void DestroySequence() {
			Destroyed = true; // Only need to be called once

			transform.parent = null; // Remove parent for free movement

			if (!transform.GetComponent<Rigidbody>()) { // Impliment physics if not already applyed
				gameObject.AddComponent<Rigidbody>();
			}

			Rigidbody Comp = transform.GetComponent<Rigidbody>(); // Shortcut (getcomponent is expensive)
			Comp.useGravity = true;
			Comp.sleepThreshold = 0.00001f; // How much movement is the minimum before it identifys as being asleep (needs more testing)

			if (AllChildStatsCode.Count > 0 && TriggerChildren) {
				AllChildStatsCode.ForEach(e => { e.DestroySequence(); });
			}

			if (DestroyOnSleep) { // whether destruction uses sleep or delay
				StartCoroutine(WaitUntillSleep());

			} else {
				DestroyOnDelay();
			}
		}

		IEnumerator WaitUntillSleep() { // Destroy when the rigid body identifys the object as asleep
			yield return new WaitUntil(gameObject.GetComponent<Rigidbody>().IsSleeping);
			Destroy(gameObject);
		}

		void DestroyOnDelay() { // Destroy after so many seconds
			Destroy(gameObject, DestroyTime);
		}

		public void ProcessDamage() { // Process any damage
			if (DamageReceived > 0) {
				Health -= DamageReceived;
				DamageReceived = 0;
			}
		}
	}

}
