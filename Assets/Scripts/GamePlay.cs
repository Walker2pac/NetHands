using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{


    public class GamePlay : MonoBehaviour
    {
        public SplineFollower splineFollower;

        public GameObject Player;

        public Transform target;
        public float speed;

        private float verticalLookOffset = -1f;

        private bool SmoothStopBool;
        private float startSpeed;
        /// <summary>
        /// 
        /// </summary>
        /// public Transform target;//follower

        private Vector3 lookPoint;//точка на которую нужно смотреть

        public float speedRot;

        private bool lookToFollower;
        private bool lookToPoint;
        private ObjectGroup objectGroup;

        private bool ignoreVerticalLook;

        private void OnEnable()
        {
            PanelStartMenu.OnStartButtonPressed += StartMove;
        }

        private void OnDisable()
        {
            PanelStartMenu.OnStartButtonPressed -= StartMove;
        }

        void Start()
        {
            startSpeed = splineFollower.followSpeed;
            splineFollower.followSpeed = 0f;

            SmoothStopBool = false;
            lookToFollower = true;
            PlayerPrefs.SetFloat("SavedSpeed", splineFollower.followSpeed);
        }
        public void StartMove()
        {
            splineFollower.followSpeed = startSpeed;
        }
        void Update()
        {
            if (!GameState.Instance.CompareState(GameState.State.Playing)) return;

            if (Player.transform.position != target.position)
            {
                Move();
            }
            /*if (SmoothStopBool)
            {
                SmoothStop();
            }*/
            if (Input.GetKeyDown("space"))
            {
                splineFollower.followSpeed = 2f;////
            }
            //////
            ///
            if (lookToFollower)
            {
                RotationFollow();
            }
            else if (lookToPoint)
            {
                LookFor();
            }

        }
        void Move()
        {
            Player.transform.position = Vector3.Lerp(Player.transform.position, target.position, speed * Time.deltaTime);
        }
        public void Stop()
        {
            if (objectGroup != null)
            {
                if (objectGroup.isComplette)
                {
                    return;
                }
            }
            else
            splineFollower.followSpeed = 0f;
        }
        /*public void StartSmoothStop()
        {
            SmoothStopBool = true;
        }*/
        /*void SmoothStop()
        {
            if (splineFollower.followSpeed >= 0f)
            {
                splineFollower.followSpeed -= (Time.deltaTime / 2);
            }
        }*/

        public void SmoothStop(float time)
        {
            if(objectGroup != null)
            {
                if (objectGroup.isComplette)
                {
                    return;
                }
                else if(time == 0)
                {
                    splineFollower.followSpeed = 0;
                }
                else
                {
                    StartCoroutine(SmoothSpeedIncrement(time));
                    Debug.Log("TriggerStop");
                }
            }
            else if (time == 0)
            {
                splineFollower.followSpeed = 0;
            }
            else
            {
                StartCoroutine(SmoothSpeedIncrement(time));
                Debug.Log("TriggerStop");
            }

        }
        public IEnumerator SmoothSpeedIncrement(float StopTime)
        {
            float t = 0f;
            float timer = StopTime;
            float Speed = splineFollower.followSpeed;
            while (t < StopTime)
            {
                    t += Time.deltaTime;
                float q = Mathf.InverseLerp(0, StopTime, t);
                float Slow = Mathf.Lerp(Speed, 0, q);
                splineFollower.followSpeed = Slow;
                yield return null;
            }
        }
        /////////
        ///
        void RotationFollow()
        {
            if (Player.transform.rotation != target.rotation)
            {
                Player.transform.rotation = Quaternion.Slerp(Player.transform.rotation, target.rotation, speedRot * Time.deltaTime);
               
            }
        }

        public void StartLookFor(Transform TargetLook)
        {
            ignoreVerticalLook = false;

            lookToFollower = false;
            lookToPoint = true;

            lookPoint = TargetLook.position;
            lookPoint.y += verticalLookOffset;
        }
        public void StopLookFor()
        {
            ignoreVerticalLook = false;

            lookToFollower = true;
            lookToPoint = false;
        }
        void LookFor()
        {
            //Player.transform.LookAt(lookPoint);
            Quaternion OriginalRot = Player.transform.rotation;
            Player.transform.LookAt(lookPoint);
            Quaternion NewRot = Player.transform.rotation;
            Player.transform.rotation = OriginalRot;
            Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, NewRot, speedRot * Time.deltaTime);

            if (ignoreVerticalLook)
            {
                Quaternion currentRotation = Player.transform.rotation;
                currentRotation.x = OriginalRot.x;
                currentRotation.z = OriginalRot.z;
                Player.transform.rotation = currentRotation;
            }
        }
        public void Finish()
        {
            splineFollower.followSpeed = 0;

            GameState.Instance.ChangeState(GameState.State.Victory);
        }
        public void ChangeSpeed(float speed)
        {
            splineFollower.followSpeed = speed;
        }

        public void Pause(float time)
        {

            float Speed = splineFollower.followSpeed;///запоминаем скорость чтобы потом ее возобновитоь
            splineFollower.followSpeed = 0f;
            StartCoroutine(ExitPause(time, Speed));
        }
        public void PauseSmooth(float PauseTime)
        {
            float Speed = splineFollower.followSpeed;///запоминаем скорость чтобы потом ее возобновитоь
            StartCoroutine(ExitPause(PauseTime, Speed));
        }
        public IEnumerator ExitPause(float tim, float speed)
        {
            while (tim > 0f)
            {
                tim -= Time.deltaTime;
                yield return null;
            }
            splineFollower.followSpeed = speed;//возобновляем движение
        }
        public void SetObjGroup(ObjectGroup MyGroup)
        {
            objectGroup = MyGroup;
            Invoke("ClearObjGroup", 0.4f);
        }
        void ClearObjGroup()
        {
            objectGroup = null;
        }

        public void IgnoreVerticalLook()
        {
            ignoreVerticalLook = true;
        }
    }

}