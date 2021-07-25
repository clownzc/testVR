using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;
/// <summary>
/// 用来测试实时生成网格的运行效率
/// </summary>
public class MyGun : MonoBehaviour
{
    [Serializable]
    public class InputConfig
    {
        public OVRInput.Button buttonShoot = OVRInput.Button.PrimaryIndexTrigger;
        public OVRInput.Button butonSwitchGun = OVRInput.Button.PrimaryHandTrigger;

    }

    [SerializeField]
    private WeaponController weaponController;
    [SerializeField]
    private InputConfig inputConfig;

    private void Start()
    {
        weaponController.Init();
    }

    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            weaponController.OnPress();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            weaponController.OnPointDown();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            weaponController.OnPointUp();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponController.SwitchShooter();
        }

         if(OVRInput.GetDown(inputConfig.buttonShoot))
         {
             weaponController.OnPointDown();
         }
         else if(OVRInput.Get(inputConfig.buttonShoot))
         {
             weaponController.OnPress();
         }
         else if (OVRInput.GetUp(inputConfig.buttonShoot))
         {
             weaponController.OnPointUp();
         }
         else if (OVRInput.GetUp(inputConfig.butonSwitchGun))
         {
             weaponController.SwitchShooter();
         }
    }

}
