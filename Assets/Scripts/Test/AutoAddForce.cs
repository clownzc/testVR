using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自动隔段时间加个力
/// </summary>
public class AutoAddForce : MonoBehaviour
{
    [SerializeField] Vector3 dir;
    [SerializeField] float timeInterval = 1;
    
    private float time;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time > timeInterval)
        {
            time = 0;
            rigidbody.AddForce(dir);
        }
    }

 

}
