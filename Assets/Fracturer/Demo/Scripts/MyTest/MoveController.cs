using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    [SerializeField] float rotationSpeed = 10;
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

    }
}
