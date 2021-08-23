using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 用来场景管理
/// </summary>
public class SceneMainManager : MonoBehaviour
{
    public enum ScenePath
    {
        Demo = 0,
        Demo1 = 1,
        Max = 2,
    }
    [Serializable]
    public class InputConfig
    {
        [Tooltip("重置场景按钮")]
        public OVRInput.Button buttonReset = OVRInput.Button.Four;
    }
    [Tooltip("设置场景切换的操控按钮")]
    [SerializeField]
    private InputConfig _inputConfig;

    private int curScene = 0;//

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(_inputConfig.buttonReset))
        {
            LoadNextScene();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        ++curScene;
        if (curScene == (int)ScenePath.Max) curScene = 0;
        Debug.Log($"Loading scene :{(ScenePath)curScene}");
        SceneManager.LoadScene(curScene);
    }
}
