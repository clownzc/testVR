using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class refereeBehavior : MonoBehaviour {
    private Animator refereeAni;
	// Use this for initialization
	void Start () {
        refereeAni = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void hitGong()
    {
        refereeAni.SetTrigger("gameBegin");
    }
    void gongHitted()
    {
        SceneAudioController.Instance.gongHit();
    }
}
