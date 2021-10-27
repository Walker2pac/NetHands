using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    float Ypos;
    void Start()
    {
        Ypos = transform.position.y;   
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, Ypos, transform.position.z);
    }
}
