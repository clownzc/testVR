using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LogoManager : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            OnStartButton();
        }
    }

    public void OnStartButton()
    {
        Debug.Log("LogoManager OnStartButton!");
        SceneManager.LoadScene(1);
    }
}
