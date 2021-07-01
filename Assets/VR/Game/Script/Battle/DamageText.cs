using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
	public class DamageText : MonoBehaviour
	{
		[SerializeField] private float _riseSpeed = 2f;

		[SerializeField] private float _duration = 1f;

		public Transform Target { get; set; }

		private Camera _camera;
		private float _currentTime;

		private void Start()
		{
			_camera = Camera.main;
			_currentTime = 0f;
		}

		private void Update()
		{
			var dt = Time.deltaTime;
			_currentTime += dt;
			if (_currentTime > _duration)
			{
				Destroy(gameObject);
				return;
			}

			transform.LookAt(transform.position + _camera.transform.forward);
			transform.position += new Vector3(0f, _riseSpeed * dt, 0f);
		}
	}
}
