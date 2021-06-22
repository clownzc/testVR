using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootBall : MonoBehaviour {

	public KeyCode shoot = KeyCode.Space;
	public float force = 1000;

	public Rigidbody ball;
	public Transform fwdDir;

	public Vector3 offsetP;
	public float R;


	void Update () {

		if (Input.GetKeyDown (shoot)) {
			Debug.Log("射！");
			ball.position = transform.TransformPoint (offsetP);
			ball.AddForce (fwdDir.forward * force);
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {

			if (R > -100) {
				R -= 1;
				transform.Rotate (0, -1, 0);
			}
		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			if (R < 100) {
				R += 1;
				transform.Rotate (0, 1, 0);
			}
		}

	}
}
