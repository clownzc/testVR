using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 10;
    [SerializeField] float camRotatespeed = 5;
    [SerializeField] Transform camTrans;
    Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Vertical") * speed;
        float v = Input.GetAxis("Horizontal") * rotationSpeed;
        h *= Time.deltaTime;
        v *= Time.deltaTime;
        transform.Translate(v, 0, h);

        //相机随着视野移动
        float X = Input.GetAxis("Mouse X") * camRotatespeed;
        float Y = Input.GetAxis("Mouse Y") * camRotatespeed;
        camTrans.localRotation = camTrans.localRotation * Quaternion.Euler(-Y, 0, 0);
        transform.localRotation = transform.localRotation * Quaternion.Euler(0, X, 0);
    }
}
