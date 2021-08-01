// Shatter Toolkit
// Copyright 2011 Gustav Olsson
using System.Collections.Generic;
using UnityEngine;

public class MouseShatter : MonoBehaviour
{
	public ShatterScheduler scheduler = null;
	private Camera mainCamera;
	private void Start()
    {
        mainCamera = Camera.main;
    }
    public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			
			if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit))
			{
				if (scheduler != null)
				{
					scheduler.AddTask(new ShatterTask(hit.collider.gameObject, hit.point));
				}
				else
				{
					hit.collider.SendMessage("Shatter", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}