using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
    public class ObjectGroup : MonoBehaviour
    {
        public GamePlay gamePlay;
        public SplineFollower splineFollower;
        public float followerSpeed;//скорость которая будет у фоловера после возобновления


        public GameObject[] Enemies = new GameObject[0];
        public GameObject[] TreatObjects = new GameObject[0];
        public GameObject[] InteractiveObjects = new GameObject[0];

        //timer
        private float nextActionTime = 0.0f;
        [HideInInspector] public float period = 2f;

        private float _changingCameraTargetTimer = 0.0f;
        [HideInInspector] public float _changingCameraTargetPeriod = 1f;
        [HideInInspector] public bool isComplette;

        public bool CameraChangeLookTarget;
        private bool _isChangingCameraTargetActive = false;
        [HideInInspector] public int DeadEnemy;

        private List<Enemy> enemiesComponents = new List<Enemy>();
        private List<IBulletInteractable> interactables = new List<IBulletInteractable>();
        private List<IMoveable> moveables = new List<IMoveable>();

        private void Awake()
        {
            foreach (GameObject enemy in Enemies)
            {
                if (enemy.TryGetComponent(out Enemy enemyComponent))
                {
                    enemiesComponents.Add(enemyComponent);
                }
            }

            interactables = new List<IBulletInteractable>(GetComponentsInChildren<IBulletInteractable>());
            moveables = new List<IMoveable>(GetComponentsInChildren<IMoveable>());
        }

        void Update()
        {
            if (!GameState.Instance.CompareState(GameState.State.Playing)) return;

            if (Time.time > nextActionTime)
            {
                nextActionTime += period;
                TaskComplette();
            }
            if (_isChangingCameraTargetActive)
            {
                if (Time.time > _changingCameraTargetTimer)
                {
                    _changingCameraTargetTimer += _changingCameraTargetPeriod;
                    ChangeLookTarget();
                }
            }
        }
        public void ChangeLookTarget()
        {
            if (CameraChangeLookTarget)
            {
                Transform closestEnemy = GetClosestEnemy();
                if (closestEnemy != null)
                {
                    gamePlay.StartLookFor(closestEnemy);
                    return;
                }

                Transform closestInteractable = GetClosestInteractable();
                if (closestInteractable != null)
                {
                    gamePlay.StartLookFor(closestInteractable);
                    return;
                }

                Transform closestInteracted = GetClosestInteracted();
                if (closestInteracted != null)
                {
                    gamePlay.StartLookFor(closestInteracted);
                    gamePlay.IgnoreVerticalLook();
                    return;
                }
            }
        }

        public void ActivateObjectGroup()
        {
            if (CameraChangeLookTarget)
            {
                _isChangingCameraTargetActive = true;
            }
            for (int i = 0; i < TreatObjects.Length; i++)
            {
                TreatObjects[i].SetActive(true);
            }
            for (int i = 0; i < InteractiveObjects.Length; i++)
            {
                if (InteractiveObjects[i].TryGetComponent(out IActivateable activateable))
                {
                    activateable.Activate();
                }
            }
            foreach (Enemy enemy in enemiesComponents)
            {
                if (enemy.IsDead || enemy.IsHooked()) continue;

                enemy.GetComponent<Move>().Init();
                enemy.objectGroup = gameObject.GetComponent<ObjectGroup>();
            }
        }
        private void TaskComplette()
        {

            foreach (Enemy enemy in enemiesComponents)
            {
                if (enemy != null)
                {
                    if (!enemy.IsDead)
                    {
                        return;
                    }
                }
            }
            for (int q = 0; q < TreatObjects.Length; q++)
            {
                if(TreatObjects != null)
                {
                    bool Check = TreatObjects[q].GetComponent<Check>().ObjectIsComplette;
                    if (!Check)
                    {                                           
                        return;
                    }
                }
                
            }
            for (int w = 0; w < InteractiveObjects.Length; w++)
            {
                if(InteractiveObjects != null)
                {
                    bool Check = InteractiveObjects[w].GetComponent<Check>().ObjectIsComplette;
                    if (!Check)
                    {
                        return;
                    }
                }
            }
            gamePlay.StopAllCoroutines();
            float LastSpeed = PlayerPrefs.GetFloat("SavedSpeed");
            if (followerSpeed != LastSpeed && followerSpeed != 0)
            {
                PlayerPrefs.SetFloat("SavedSpeed", followerSpeed);
                splineFollower.followSpeed = followerSpeed;
                
            }
            else
            {
                followerSpeed = LastSpeed;
                splineFollower.followSpeed = followerSpeed;
            }

            StopAllCoroutines();
            isComplette = true;
            gameObject.GetComponent<ObjectGroup>().enabled = false;
        }

        private float GetSqrDistanceToPlayer(Vector3 point)
        {
            return (splineFollower.transform.position - point).sqrMagnitude;
        }

        private bool IsAllEnemiesHooked()
        {
            bool answer = true;

            foreach (Enemy enemy in enemiesComponents)
            {
                if (!enemy.IsHooked())
                {
                    answer = false;
                    break;
                }
            }

            return answer;
        }

        private Transform GetClosestEnemy()
        {
            Transform lookTarget = GetLookTarget(IsAllEnemiesHooked());

            if (lookTarget != null)
            {
                //Debug.Log($"Distance to closest enemy is {distance}");
                Debug.DrawLine(splineFollower.transform.position, lookTarget.position, Color.cyan, 5f);
                Debug.DrawLine(lookTarget.position, lookTarget.position + Vector3.up * 3f, Color.cyan, 5f);
            }

            return lookTarget;
        }

        private Transform GetClosestInteractable()
        {
            float distance = float.PositiveInfinity;
            Transform lookTarget = null;

            foreach (IBulletInteractable interactable in interactables)
            {
                if (!interactable.IsInteractable) continue;

                float distanceToPlayer = GetSqrDistanceToPlayer(interactable.GetTransform().position);
                if (distanceToPlayer < distance)
                {
                    lookTarget = interactable.GetTransform();
                    distance = distanceToPlayer;
                }
            }

            return lookTarget;
        }

        private Transform GetClosestInteracted()
        {
            float distance = float.PositiveInfinity;
            Transform lookTarget = null;

            foreach (IMoveable moveable in moveables)
            {
                if (!moveable.IsMoving) continue;

                float distanceToPlayer = GetSqrDistanceToPlayer(moveable.GetTransform().position);
                if (distanceToPlayer < distance)
                {
                    lookTarget = moveable.GetTransform();
                    distance = distanceToPlayer;
                }
            }

            return lookTarget;
        }

        private Transform GetLookTarget(bool includeHooked)
        {
            float distance = float.PositiveInfinity;
            Transform lookTarget = null;

            foreach (Enemy enemy in enemiesComponents)
            {
                if (!includeHooked && enemy.IsHooked()) continue;

                if (!enemy.IsDead)
                {
                    BodyPart bodyPart = enemy.HeadBodyPart;
                    if (bodyPart != null)
                    {
                        float distanceToPlayer = GetSqrDistanceToPlayer(bodyPart.transform.position);
                        if (distanceToPlayer < distance)
                        {
                            lookTarget = bodyPart.transform;
                            distance = distanceToPlayer;
                        }
                    }
                    else
                    {
                        Debug.LogError("HeadBodyPart not assigned");
                    }
                }
            }

            return lookTarget;
        }
    }
}
