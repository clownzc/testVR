
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEntry : MonoBehaviour
{
	[SerializeField]
	private GameObject _noVRGameObject;

	[SerializeField] 
	private GameObject _steamVRGameObject;
	
	private void Start()
	{
		if (SteamVR.active)
		{
			_noVRGameObject.SetActive(false);
			_steamVRGameObject.SetActive(true);
		}
		else
		{
			_noVRGameObject.SetActive(true);
			_steamVRGameObject.SetActive(false);
		}
	}
}
