using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public GameObject Gun;
    public GameObject Palka;

    public Enemy enemy;
    public EnemyAnimator enemyAnimator;
    public EnemyShoot enemyShoot;
    public Move move;
    public GameObject TargetFriend;
   

    public float SpeedMove;
    public float AttackDelayTime;
    public float DelayBeforeAttack;
    [SerializeField] private float delayBeforeAttackAnimation = 0f;
    [SerializeField] private float distanceOfAttack = 1.6f;
    [SerializeField] private bool useJumpOnAttack = false;
    [SerializeField] private float verticalOffsetAfterJump = 0f;
    [SerializeField] private float jumpSpeed = 15f;
    [SerializeField] private float verticalJumpOffset = 2f;
    public enum State {none,normal,attack};
    public State enemyState;
    private Vector3 TargetPos;

    private IEnumerator moveCoroutine;

    public void StartAttack()
    {
        if (enemyState == State.attack)
        {
            Debug.Log("Start");
            move.StopAllCoroutines();
            move.enabled = false;
            transform.LookAt(TargetFriend.transform.position);

            if (GameState.Instance.CompareState(GameState.State.Playing))
            {
                CheckType();
            }
            else
            {
                GameState.Instance.OnPlaying += GameStateOnPlaying;
            }
        }
    }

    public void StopMovement()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
    }

    private void GameStateOnPlaying()
    {
        CheckType();
        GameState.Instance.OnPlaying -= GameStateOnPlaying;
    }

    void CheckType()
    {
        StopMovement();

        if (enemyAnimator.ShootBool)
        {
            enemyShoot.Shoot(TargetFriend.transform,AttackDelayTime);
            if(TargetFriend != null)
            {
                if(TargetFriend.GetComponent<Friend>()!= null)
                {
                    TargetFriend.GetComponent<Friend>().Pray();
                }
            }
            Gun.SetActive(true);
            enemyAnimator.AnimAttack();

        }
        if (enemyAnimator.SwoordBool)
        {
            moveCoroutine = MoveToTarget(TargetFriend.transform);
            StartCoroutine(moveCoroutine);
            Palka.SetActive(true);
        }
        if (enemyAnimator.PushBool)
        {
            moveCoroutine = MoveToTarget(TargetFriend.transform);
            StartCoroutine(moveCoroutine);
        }
    }
    IEnumerator MoveToTarget(Transform Target)
    {
        enemy.FaceAtack();
        enemyAnimator.AnimIdle();
        yield return new WaitForSeconds(AttackDelayTime);

        Invoke(nameof(Fearr), 1f);
        enemyAnimator.AnimRun();

        while (Vector3.Distance(transform.position, TargetPos) > distanceOfAttack)
        {
            TargetPos = new Vector3(Target.position.x, transform.position.y, Target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, TargetPos, SpeedMove * Time.deltaTime);
            Vector3 lookPosition = TargetFriend.transform.position;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);
            yield return null;
        }
        enemyAnimator.AnimIdle();

        yield return new WaitForSeconds(DelayBeforeAttack);
        enemyAnimator.AnimAttack();
        yield return new WaitForSeconds(delayBeforeAttackAnimation);

        if (useJumpOnAttack && enemyAnimator.attackAnimationType == EnemyAnimator.AttackAnimationType.push)
        {
            Vector3 startPosition = transform.position;
            Vector3 endPoint = TargetPos - ((TargetPos - transform.position).normalized * 1.5f);
            endPoint.y += verticalOffsetAfterJump;
            Vector3 curvePoint = Vector3.Lerp(startPosition, endPoint, 0.5f);
            curvePoint.y += verticalJumpOffset;
            float time = 0.5f;
            float currentTime = 0f;
            while (currentTime <= time)
            {
                Vector3 m1 = Vector3.Lerp(startPosition, curvePoint, currentTime / time);
                Vector3 m2 = Vector3.Lerp(curvePoint, endPoint, currentTime / time);
                transform.position = Vector3.Lerp(m1, m2, currentTime / time);

                Vector3 lookPosition = TargetFriend.transform.position;
                lookPosition.y = transform.position.y + verticalOffsetAfterJump;
                transform.LookAt(lookPosition);

                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPoint;
            SetDamage();
        }
        else
        {
            Invoke(nameof(SetDamage), 0.8f);
        }
    }
    void SetDamage()
    {
        IHittableByEnemy hittableByEnemy = TargetFriend.GetComponent<IHittableByEnemy>();
        if (hittableByEnemy != null)
        {
            hittableByEnemy.TakeDamage();
            hittableByEnemy.EnemyHit();
        }
    }
   
    void Fearr()//Strax
    {
        if (TargetFriend.GetComponent<Friend>() != null)
        {
            TargetFriend.GetComponent<Friend>().Fear();
        }
    }

    public void MoveToPoint(Vector3 point)
    {
        StopMovement();
        moveCoroutine = MoveToPointCoroutine(point);
        StartCoroutine(moveCoroutine);
    }

    IEnumerator MoveToPointCoroutine(Vector3 point)
    {
        enemyAnimator.AnimRun();
        Vector3 endPoint = point;

        while (Vector3.Distance(transform.position, endPoint) > 0f)
        {
            endPoint = new Vector3(endPoint.x, transform.position.y, endPoint.z);
            transform.position = Vector3.MoveTowards(transform.position, endPoint, SpeedMove * Time.deltaTime);
            Vector3 lookPosition = endPoint;
            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition);
            yield return null;
        }
        enemyAnimator.AnimIdle();
    }
}
