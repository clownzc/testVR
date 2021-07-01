using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorsControlle : MonoBehaviour {
    public GameObject door01;
    public GameObject door02;
    public GameObject door03;
    public GameObject door04;
    public GameObject door05;
    public GameObject door06;
    private Animator ani01;
    private Animator ani02;
    private Animator ani03;
    private Animator ani04;
    private Animator ani05;
    private Animator ani06;

    // Use this for initialization
    void Start () {
        ani01 = door01.GetComponent<Animator>();
        ani02 = door02.GetComponent<Animator>();
        ani03 = door03.GetComponent<Animator>();
        ani04 = door04.GetComponent<Animator>();
        ani05 = door05.GetComponent<Animator>();
        ani06 = door06.GetComponent<Animator>();
    }
	// Update is called once per frame
	void Update () {

    }
    void doorControlle(int doorIndex, string behavior)
    {
        if (doorIndex == 1)
        {
            if(behavior == "open")
            {
                ani01.SetTrigger("openDoor");
            }else if(behavior == "close")
            {
                ani01.SetTrigger("closeDoor");
            }
                doorAudio(door01.transform.position);
        }
        if (doorIndex == 2)
        {
            if (behavior == "open")
            {
                ani02.SetTrigger("openDoor");
            }
            else if (behavior == "close")
            {
                ani02.SetTrigger("closeDoor");
            }
            doorAudio(door02.transform.position);

        }
        if (doorIndex == 3)
        {
            if (behavior == "open")
            {
                ani03.SetTrigger("openDoor");
            }
            else if (behavior == "close")
            {
                ani03.SetTrigger("closeDoor");
            }
            doorAudio(door03.transform.position);

        }
        if (doorIndex == 4)
        {
            if (behavior == "open")
            {
                ani04.SetTrigger("openDoor");
            }
            else if (behavior == "close")
            {
                ani04.SetTrigger("closeDoor");
            }
            doorAudio(door04.transform.position);

        }
        if (doorIndex == 5)
        {
            if (behavior == "open")
            {
                ani05.SetTrigger("openDoor");
            }
            else if (behavior == "close")
            {
                ani05.SetTrigger("closeDoor");
            }
            doorAudio(door05.transform.position);

        }
        if (doorIndex == 6)
        {
            if (behavior == "open")
            {
                ani06.SetTrigger("openDoor");
            }
            else if (behavior == "close")
            {
                ani06.SetTrigger("closeDoor");
            }
            doorAudio(door06.transform.position);

        }
    }
    void doorAudio(Vector3 position)
    {
        SceneAudioController.Instance.doorMove(position);
    }
}
