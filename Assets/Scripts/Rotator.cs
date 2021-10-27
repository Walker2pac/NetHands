using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Transform target;//follower

    private Vector3 lookPoint;//точка на которую нужно смотреть

    public float speedRot;

    private bool lookToFollower;
    private bool lookToPoint;
    void Start()
    {
        lookToFollower = true;
    }
    void Update()
    {
        if (lookToFollower)
        {
            RotationFollow();
        }
        else if (lookToPoint)
        {
            LookFor();
        }
    }
    void RotationFollow()
    {
        if(transform.rotation != target.rotation)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, speedRot * Time.deltaTime);
        }
    }
    public void StartLookFor(Transform TargetLook)
    {
        lookToFollower = false;
        lookToPoint = true;

        lookPoint = TargetLook.position;
    }
    public void StopLookFor()
    {
        lookToFollower = true;
        lookToPoint = false;
    }
    void LookFor()
    {
        transform.LookAt(lookPoint);
    }
}
