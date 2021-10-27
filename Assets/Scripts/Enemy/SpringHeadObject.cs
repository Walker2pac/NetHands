using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringHeadObject : MonoBehaviour
{
    private Transform _target;
    private SpringJoint _springJoint;
    private float offsetMultiplier = 1f;
    private float speed = 0.7f;
    private float t;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetSpringJoint(SpringJoint springJoint)
    {
        _springJoint = springJoint;
    }

    void Update()
    {
        if (_springJoint != null)
        {
            t = Mathf.PingPong(Time.time * speed, 1f);
            _springJoint.spring = Mathf.Lerp(100f, 7000f, t);
        }

        if (_target == null) return;

        transform.position = _target.transform.position + Vector3.up * offsetMultiplier;
    }
}
