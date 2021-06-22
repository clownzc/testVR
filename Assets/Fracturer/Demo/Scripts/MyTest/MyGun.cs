using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bytesized;
/// <summary>
/// 用来测试实时生成网格的运行效率
/// </summary>
public class MyGun : MonoBehaviour
{
    public LayerMask ShardLayers;
    public float ExplosionRadius;
    public float ExplosionForce;

    [SerializeField]
    private Camera myCamera;
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
            RaycastHit hit;
            var ray = new Ray(myCamera.transform.position, myCamera.transform.forward);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {              
                weaponController.Shoot(transform.position, hit.point, hit.collider.gameObject);
               // Explode(hit.point);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponController.SwitchShooter();
        }
    }
}
