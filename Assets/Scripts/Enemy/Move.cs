using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyState))]
public class Move : MonoBehaviour
{
    public EnemyAnimator enemyAnimator;

    public enum MoveType { None, Static,OneSide,PingPong,Circle};
    public MoveType moveType;

    public GameObject[] MovePoints = new GameObject[0];
    public Transform LookTarget;
    private bool ChangeLookFromPoint;

    public float SpeedMove;
    public float DelayTime;
    private int _currentPoint = 0;

    private bool PingPongForward = true;
    private MovePoint movePoints;
    public bool MoveOnStart;

    [Space]
    [Header("Attacking")]
    [SerializeField] private bool isAttackingAfterReachingLastPoint = false;
    [SerializeField] private float delayBeforeAttack = 0f;
    private EnemyState enemyState;

    private void Awake()
    {
        enemyState = GetComponent<EnemyState>();
    }

    void Start()
    {
        if (MoveOnStart)
        {
            if (moveType != MoveType.Static)
            {
                StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
            }
        } 
       
       
    }
    public void Init()
    {
        if (moveType != MoveType.Static)
        {
            StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
        }
        else
        {
            enemyState.StartAttack();
        }
    }


    IEnumerator MoveTo(Vector3 target)
    {
        
        yield return new WaitForSeconds(DelayTime);

        if (ChangeLookFromPoint)
        {
            LookTarget = movePoints.TargetLook;
        }
        else
        {
            LookTarget = MovePoints[_currentPoint].transform;
        }
        ChangeLookFromPoint = false;
        enemyAnimator.AnimRun();
        while (transform.position != MovePoints[_currentPoint].transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, MovePoints[_currentPoint].transform.position, SpeedMove * Time.deltaTime);
            Rotate(LookTarget);
            yield return null;
        }
        ChangeCheck();
        FindNextPoint();

        if (moveType == MoveType.OneSide)
        {
            enemyAnimator.AnimIdle();
        }
    }
    void FindNextPoint()
    {
        if(moveType == MoveType.OneSide)
        {
            if(_currentPoint < MovePoints.Length-1)
            {
                _currentPoint++;
                StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
            }
            else if (isAttackingAfterReachingLastPoint)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
        else if (moveType == MoveType.PingPong)
        {
            if (PingPongForward)
            {
                if(_currentPoint < MovePoints.Length-1)
                {
                    _currentPoint++;
                    StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
                }
                else if(_currentPoint == MovePoints.Length-1)
                {
                    PingPongForward = false;
                    _currentPoint--;
                    StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
                }
                
            }
            else
            {
                if(_currentPoint > 0)
                {
                    _currentPoint--;
                    StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
                }
                else if (_currentPoint == MovePoints.Length-1)
                {
                   
                    _currentPoint--;
                    StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
                }
                else if (_currentPoint == 0)
                {
                    PingPongForward = true;
                    _currentPoint++;
                    StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
                }
            }

        }
        else if(moveType == MoveType.Circle)
        {
            if (_currentPoint < MovePoints.Length-1)
            {
                _currentPoint++;
                StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
            }
            else
            {
                _currentPoint = 0;
                StartCoroutine(MoveTo(MovePoints[_currentPoint].transform.position));
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(delayBeforeAttack);

        enemyState.StartAttack();
    }
    
    void Rotate(Transform Target)
    {
        Quaternion OriginalRot = transform.rotation;
        transform.LookAt(Target);
        Quaternion NewRot = transform.rotation;
        transform.rotation = OriginalRot;
        transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, 2 * Time.deltaTime);
    }
    void ChangeCheck()
    {   
        if(MovePoints[_currentPoint].gameObject.GetComponent<MovePoint>() != null)
        {
            movePoints = MovePoints[_currentPoint].GetComponent<MovePoint>();
            if (movePoints.ChangeDelay)
            {
                DelayTime = movePoints.Delay;
                enemyAnimator.AnimIdle();
            }
            if (movePoints.ChangeSpeed)
            {
                SpeedMove = movePoints.ChangedSpeed;
            }
            if (movePoints.ChangeLook)
            {
                ChangeLookFromPoint = true;
            }
        }
    }
}
