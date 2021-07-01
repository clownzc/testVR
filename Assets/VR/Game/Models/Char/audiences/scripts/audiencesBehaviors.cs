using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiencesBehaviors : MonoBehaviour {
    public Animator audienceController; 
	// Use this for initialization
	void Start () {
        audienceController = GetComponent<Animator>();
        setAnimation(1);
	}

    void OnEnable()
    {
        Messenger.AddListener("KingAction", OnKingAction);
        Messenger.AddListener<int>("CrowdAction", OnCrowdAction);
    }
    
    void OnDisable()
    {
        Messenger.RemoveListener("KingAction", OnKingAction);
        Messenger.RemoveListener<int>("CrowdAction", OnCrowdAction);
    }

    private void OnKingAction()
    {
        setKingAnimation();
    }

    private void OnCrowdAction(int action)
    {
        setAnimation(action);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.W))
        {
            setAnimation(3);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            setAnimation(2);
            setKingAnimation();
        }

    }
    void setAnimation(int behavior)
    {
        audienceController.SetInteger("behavior", behavior);
        int action = Random.Range(1,6);
        audienceController.SetInteger("idleAni", action);
        audienceController.SetInteger("actionAni", action);
        audienceController.SetInteger("cheerAni", action);
        if(behavior == 3)
        {
            cheerVoice();
        }
        if(behavior == 2)
        {
            actionVoice();
        }
    }
    void setKingAnimation()
    {
        if (this.name == "audienceKing")
        {
            audienceController.SetInteger("behavior", 2);
            audienceController.SetInteger("actionAni", 4);
            kingAnnounce("begin");
        }
            
    }

    void kingActionVoice()
    {
        if(this.name == "audienceKing")
        {
            SceneAudioController.Instance.voice();
        }
    }
    void actionVoice()
    {
        SceneAudioController.Instance.action();
    }
    void cheerVoice()
    {
        SceneAudioController.Instance.cheer();
    }

    void kingAnnounce(string action)
    {
        if(action == "begin")
        {
            SceneAudioController.Instance.beginTalk();
        }
        else if(action == "end"){
            SceneAudioController.Instance.endTalk();
        }
    }
}
