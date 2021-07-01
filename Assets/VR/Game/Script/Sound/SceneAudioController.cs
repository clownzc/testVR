using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudioController : MonoBehaviour {
    private static SceneAudioController m_instance;
    private AudioSource gongAudio;
    private AudioSource doorAudio;
    private AudioSource audienceVoice;
    private AudioSource audienceCheer;
    private AudioSource audienceAction;

    private AudioSource voiceBegin;
    private AudioSource voiceEnd;


    public static SceneAudioController Instance
    {
        get
        {
            return m_instance;
        }

        set
        {
            m_instance = value;
        }
    }

    // Use this for initialization
    void Start () {
        gongAudio = transform.Find("gongAudio").GetComponent<AudioSource>();
        doorAudio = transform.Find("doorAudio").GetComponent<AudioSource>();
        audienceVoice = transform.Find("voiceTalk").GetComponent<AudioSource>();
        audienceCheer = transform.Find("voiceCheer").GetComponent<AudioSource>();
        audienceAction = transform.Find("voiceAction").GetComponent<AudioSource>();
        voiceBegin = transform.Find("voiceBegin").GetComponent<AudioSource>();
        voiceEnd = transform.Find("voiceEnd").GetComponent<AudioSource>();
        m_instance = this;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void gongHit()
    {
        gongAudio.Play();
    }
    public void doorMove(Vector3 position)
    {
        if (!doorAudio.isPlaying)
        {
            doorAudio.Play();
        }
        doorAudio.transform.position = position;
    }
    public void voice()
    {
        audienceVoice.Play();
    }
    public void cheer()
    {
        audienceCheer.Play();
    }
    public void action()
    {
        audienceAction.Play();
    }

    public void beginTalk()
    {
        voiceBegin.Play();
    }
    public void endTalk()
    {
        voiceEnd.Play();
    }
}
