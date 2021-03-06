using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
/// <summary>
/// 用来抓起木偶型敌人
/// </summary>
public class PuppetGrab : MonoBehaviour
{
	[Serializable]
	public class InputConfig
	{
		public OVRInput.Button buttonGrabing = OVRInput.Button.One;
	}
	[Tooltip("The layers we wish to grab (optimization).")]
	public LayerMask grabLayer;
	[SerializeField] Transform anchor;//跟随的物体
	[SerializeField] InputConfig inputConfig;

	private bool grabing;//正在尝试抓
	private bool grabbed;
	private Rigidbody r;
	private Collider c;
	private BehaviourPuppet otherPuppet;
	private Collider otherCollider;
	private ConfigurableJoint joint;
	private float nextGrabTime;

	private const float massMlp = 5f;
	private const int solverIterationMlp = 10;

	void Start()
	{
		r = GetComponent<Rigidbody>();
		c = GetComponent<Collider>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (grabing == false) return;
		if (grabbed) return; // If we have not grabbed anything yet...
		if (Time.time < nextGrabTime) return; // ...and enough time has passed since the last release...
		if (LayerUtil.IsInLayerMask(collision.collider.gameObject, grabLayer) == false) return; // ...and the collider is on the right layer...
		if (collision.rigidbody == null) return; // ...and it has a rigidbody attached.
		// Find MuscleCollisionBroadcaster that is a component added to all muscles by the PM, it broadcasts collisions events to PM and its behaviours.
		var m = collision.collider.gameObject.GetComponent<MuscleCollisionBroadcaster>();
		if (m == null) return; // Make sure the collider we collided with is a muscle of a Puppet...
	
		// Unpin the puppet we collided with
		foreach (BehaviourBase b in m.puppetMaster.behaviours)
		{
			if (b is BehaviourPuppet)
			{
				otherPuppet = b as BehaviourPuppet;
				otherPuppet.SetState(BehaviourPuppet.State.Unpinned); // Unpin
				otherPuppet.canGetUp = false; // Make it not get up while being held
			}
		}

		if (otherPuppet == null) return; // If not BehaviourPuppet found, break out

		// Adding a ConfigurableJoint to link the two puppets
		joint = gameObject.AddComponent<ConfigurableJoint>();
		joint.connectedBody = collision.rigidbody;

		// Move the anchor to where the hand is (since we done have a rigidbody for the hand)
		joint.anchor = new Vector3(-0.35f, 0f, 0f);

		// Lock linear and angular motion of the joint
		joint.xMotion = ConfigurableJointMotion.Locked;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;

		// Increasing the mass of the linked Rigidbody when it is a part of a chain is the easiest way to improve link stability.
		r.mass *= massMlp;
	
		// Ignore collisions with the object we grabbed
		otherCollider = collision.collider;
		Physics.IgnoreCollision(c, otherCollider, true);

		// We have successfully grabbed the other puppet
		grabbed = true;
	}

	void Update()
	{
		transform.position = anchor.position;
		transform.rotation = anchor.rotation;
		if (Input.GetKeyDown(KeyCode.X))
		{
			grabing = true;
		}
        else if (Input.GetKeyUp(KeyCode.X))
        {
            grabing = false;
            ReleasePuppet();
		}

        if (OVRInput.GetDown(inputConfig.buttonGrabing))
		{
			grabing = true;
		}
		else if (OVRInput.GetUp(inputConfig.buttonGrabing))
		{
			grabing = false;
			ReleasePuppet();
		}
	}

	/// <summary>
	/// 放下
	/// </summary>
	private void ReleasePuppet()
	{
		if (grabbed == false) return;
		Destroy(joint);
		r.mass /= massMlp;
		Physics.IgnoreCollision(c, otherCollider, false);
		otherPuppet.canGetUp = true;
		otherPuppet = null;
		otherCollider = null;
		grabbed = false;
		nextGrabTime = Time.time + 1f;
	}
}
