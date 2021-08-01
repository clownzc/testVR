// Shatter Toolkit
// Copyright 2011 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

public class MouseInstantiate : MonoBehaviour
{
	public GameObject prefabToInstantiate;
	private Camera mainCamera;
	public float speed = 7.0f;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void Update()
	{
		if (Input.GetMouseButtonDown(0) && prefabToInstantiate != null)
		{
			Ray mouseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
			
			GameObject newGameObject = (GameObject)Instantiate(prefabToInstantiate, mouseRay.origin, Quaternion.identity);
			var rigidbody = newGameObject.GetComponent<Rigidbody>();
			if (rigidbody != null)
			{
				rigidbody.velocity = mouseRay.direction * speed;
			}
		}
	}
}