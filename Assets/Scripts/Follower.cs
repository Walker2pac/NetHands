using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{


    public class Follower : MonoBehaviour
    {
        public SplineFollower splineFollower;
    
        public Transform target;

        public float speed;
        private bool SmoothStopBool;
        void Start()
        {
            SmoothStopBool = false;
        }
        void Update()
        {
            if (transform.position != target.position)
            {
                Move();
            }
            if (SmoothStopBool)
            {
                SmoothStop();
            }
            if (Input.GetKeyDown("space"))
            {
                splineFollower.followSpeed = 2f;////
            }
           
        }
        void Move()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        }
        public void Stop()
        {
            splineFollower.followSpeed = 0f;
        }
        public void StartSmoothStop()
        {
            SmoothStopBool = true;
        }
        void SmoothStop()
        {
            if (splineFollower.followSpeed >= 0f)
            {
                splineFollower.followSpeed -= (Time.deltaTime / 2);
            }
        }
        public void WaitFor()
        {
            splineFollower.followSpeed = 0f;
        }
        public void SmoothWaitFor(float time)
        {
            StartCoroutine(SmoothSpeedIncrement(time));
        }
        IEnumerator SmoothSpeedIncrement(float StopTime)
        {
            float t = 0f;
            float timer = StopTime;
            float Speed = splineFollower.followSpeed;
            while (t < StopTime)
            {
                t+= Time.deltaTime;
                float q = Mathf.InverseLerp(0, StopTime,t);
                float Slow = Mathf.Lerp(Speed, 0, q);
                splineFollower.followSpeed = Slow;
                Debug.Log(Slow);
                yield return null;
            }
        }

    }

}