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
    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(_inputConfig.buttonReset))
        {
            LoadScene(ScenePath.Demo);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadScene(ScenePath.Demo);
        }
    }

    private void LoadScene(ScenePath scene)
    {
        SceneManager.LoadScene((int)scene);
    }
}
