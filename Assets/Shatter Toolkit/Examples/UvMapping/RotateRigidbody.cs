// Shatter Toolkit
// Copyright 2011 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RotateRigidbody : MonoBehaviour
{
	public Vector3 axis = Vector3.up;
	
	public float angularVelocity = 7.0f;

	private Rigidbody rigidbody;

	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		Quaternion deltaRotation = Quaternion.AngleAxis(angularVelocity * Time.fixedDeltaTime, axis);
		
		rigidbody.MoveRotation(rigidbody.rotation * deltaRotation);
	}
}