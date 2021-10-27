using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public enum AttackAnimationType { none, sword, push, shoot };


public class EnemyAnimator : MonoBehaviour
{
    public enum IdleAnimationType { none, idle, idleTwo, idleThree }
    public IdleAnimationType idleAnimationType;

    public enum RunAnimationType { none, run, runTwo, runThird, runFour }
    public RunAnimationType runAnimationType;

    public enum AttackAnimationType { none, sword, push, shoot }
    public AttackAnimationType attackAnimationType;

    public bool ShootBool, SwoordBool, PushBool;

    public Animator animator;

    [SerializeField] private Transform[] bones;
    [SerializeField] private Vector3[] ragdollPositionsBones;
    [SerializeField] private Quaternion[] ragdollQuaternionsBones;
    [SerializeField] private float blendSpeed;
    [SerializeField] private float blendTime = 1f;
    private EnemyAnimatorTimer enemyAnimatorTimer;
    private bool isEnabling = false;

    private void Awake()
    {
        ragdollPositionsBones = new Vector3[bones.Length];
        ragdollQuaternionsBones = new Quaternion[bones.Length];

        if (attackAnimationType == AttackAnimationType.sword)
        {
            SwoordBool = true;
        }
        else if (attackAnimationType == AttackAnimationType.shoot)
        {
            ShootBool = true;
        }
        else if (attackAnimationType == AttackAnimationType.push)
        {
            PushBool = true;
        }
    }
    void Start()
    {
        AnimIdle();
    }

    public void AnimIdle()
    {
        if (idleAnimationType == IdleAnimationType.idle)
        {
            animator.SetTrigger("idle");
        }
        if (idleAnimationType == IdleAnimationType.idleTwo)
        {
            animator.SetTrigger("idleTwo");
        }
        if (idleAnimationType == IdleAnimationType.idleThree)
        {
            animator.SetTrigger("idleThree");
        }
    }
    public void AnimRun()
    {
        if (runAnimationType == RunAnimationType.run)
        {
            animator.SetTrigger("run");
        }
        if (runAnimationType == RunAnimationType.runTwo)
        {
            animator.SetTrigger("runTwo");
        }
        if (runAnimationType == RunAnimationType.runThird)
        {
            animator.SetTrigger("runThird");
        }
        if (runAnimationType == RunAnimationType.runFour)
        {
            animator.SetTrigger("runFour");
        }
    }
    public void AnimAttack()
    {

        if (attackAnimationType == AttackAnimationType.sword)
        {
            animator.SetTrigger("sword");
        }
        else if (attackAnimationType == AttackAnimationType.shoot)
        {
            animator.SetTrigger("shoot");
        }
        else if (attackAnimationType == AttackAnimationType.push)
        {
            animator.SetTrigger("push");
        }
    }

    public void Disable()
    {
        animator.enabled = false;
        isEnabling = false;
        enemyAnimatorTimer = null;
    }
   
    public void Enable()
    {
        for (int i = 0; i < bones.Length; i++)
        {
            ragdollPositionsBones[i] = bones[i].localPosition;
            ragdollQuaternionsBones[i] = bones[i].rotation;
        }

        animator.enabled = true;

        enemyAnimatorTimer = new EnemyAnimatorTimer(blendTime);
        enemyAnimatorTimer.Start();
    }

    private void LateUpdate()
    {
        if (enemyAnimatorTimer == null || !enemyAnimatorTimer.IsActive()) return;

        enemyAnimatorTimer.Execute(() =>
        {
            float t = enemyAnimatorTimer.GetT();
            for (int i = 0; i < bones.Length; i++)
            {
                bones[i].localPosition = Vector3.Lerp(ragdollPositionsBones[i], bones[i].localPosition, t);
                bones[i].rotation = Quaternion.Slerp(ragdollQuaternionsBones[i], bones[i].rotation, t);
            }
        });
    }
}

public class EnemyAnimatorTimer
{
    private float _currentTime = 0f;
    private float _time;
    private bool _isActive = false;
    public EnemyAnimatorTimer(float time)
    {
        _time = time;
    }

    public void Start()
    {
        _isActive = true;
        _currentTime = 0f;
    }

    public void Execute(Action action)
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= _time)
        {
            _isActive = true;
            return;
        }

        action?.Invoke();
    }

    public float GetT()
    {
        return _currentTime / _time;
    }

    public bool IsActive()
    {
        return _isActive;
    }
}

