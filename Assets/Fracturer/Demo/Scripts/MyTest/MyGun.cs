using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;
/// <summary>
/// 用来测试实时生成网格的运行效率
/// </summary>
public class MyGun : MonoBehaviour
{
    [SerializeField]
    private WeaponController weaponController;

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

         if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
         {
             weaponController.OnPointDown();
         }
         else if(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
         {
             weaponController.OnPress();
         }
         else if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
         {
             weaponController.OnPointUp();
         }
         else if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickDown))
         {
             weaponController.SwitchShooter();
         }
    }

}
