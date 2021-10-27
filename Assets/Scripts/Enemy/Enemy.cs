using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using NetHands.Enemy;
using System;

public class Enemy : MonoBehaviour, IWebable, IHookable, IDeadable, IWebConditionChecked
{
    [Header("Face Objects")]
    public GameObject[] Eyes = new GameObject[3];
    public GameObject[] Mounts = new GameObject[2];
    [Space]

    [Header("Scripts")]
    public ObjectGroup objectGroup;
    public Check check;
    public EnemyShoot enemyShoot;
    public EnemyAnimator enemyAnimator;
    public EnemyState enemyState;
    [SerializeField] private Animator animator;
    [Space]

    [Header("Render")]
    public Material GrayMaterial;
    public SkinnedMeshRenderer meshRenderer;

    [Header("Physicks")]
    public Rigidbody RbPelvis;
    [SerializeField] private Collider[] _collidersForFindingFarthestPoint;
    public CapsuleCollider[] Foots = new CapsuleCollider[0];
    private List<Rigidbody> RbChild;
    [SerializeField] private Rigidbody headRigidbody;
    private GameObject headJointObject;
    private SpringHeadObject springHeadObject;

    private GameObject bullet;
    private Transform FollowChildTransform;
    private bool follow;
    private bool bulletTouchMe;
    private float _bulletCollisionAmount;
    public float shootToDeath;

    private BodyPart[] bodyParts;
    [SerializeField] private BodyPart headBodyPart;
    public BodyPart HeadBodyPart => headBodyPart;

    [Space]
    [SerializeField] private EnemyWebActivateCondition[] enemyWebActivateConditions;

    [Space]
    [Header("Death")]
    [SerializeField] private GameObject[] AcivateObjectsOnDeath;

    [Space]
    [Header("Hooked behaviour")]
    [SerializeField] private bool canSelfUnhook = false;
    [SerializeField] private float timeToSelfUnhook = 2f;

    private bool isKilledAfterHitDelay = true;
    private float timeToKillAfterHitDelay = 15f;
    private EnemyKillTimer enemyKillTimer;

    private Vector3 startPoint;

    public bool IsDead => check.ObjectIsComplette;

    private void Awake()
    {
        startPoint = transform.position;

        RbChild = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        if (TryGetComponent(out Rigidbody thisRigidbody))
        {
            RbChild.Remove(thisRigidbody);
        }
        bodyParts = GetComponentsInChildren<BodyPart>();
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.Init(this, this, this, this);
        }

        enemyKillTimer = new EnemyKillTimer(timeToKillAfterHitDelay, Kill);
    }

    void Start()
    {
        if (headJointObject == null && headRigidbody != null)
        {
            CreateHeadJoint();
        }

        for (int i = 0; i < RbChild.Count; i++)
        {
            RbChild[i].isKinematic = true;
        }
    }

    void Update()
    {
        if (follow)
        {
            transform.position = Vector3.Lerp(transform.position, FollowChildTransform.position, 10f * Time.deltaTime);
        }
        enemyKillTimer.Tick(Time.deltaTime);
    }
    public void BulletTouch()
    {
        bulletTouchMe = true;
        
    } 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stena"))
        {
            StenaCollision(other.gameObject);
        }
    }

    public void Hit(GameObject bulletObject, float damage = 1f)
    {
        if (damage >= 0)
        {
            _bulletCollisionAmount += damage;
        }
        else
        {
            _bulletCollisionAmount++;
        }
        FaceBulletCollision();
        if (_bulletCollisionAmount >= shootToDeath)
        {
            enemyKillTimer.Stop();
            Kill();
        } 
        else
        {
            enemyKillTimer.Start();
        }
        bullet = bulletObject.gameObject;
        if (objectGroup != null)
        {
            objectGroup.ChangeLookTarget();
        }
        enemyShoot.StopAllCoroutines();
        enemyShoot.enabled = false;
        enemyShoot.StopShooting();

        if (canSelfUnhook)
        {
            Invoke(nameof(SelfUnhook), timeToSelfUnhook);
        }
    }

    private void ActivateOnDeathObjects()
    {
        foreach (GameObject AcivateObjectOnDeath in AcivateObjectsOnDeath)
        {
            AcivateObjectOnDeath.SetActive(true);
        }
    }

    public void Kill()
    {
        FaceDeath();
        Invoke(nameof(ChangeColor), 0.4f);
        EnabledCheck();
        if (objectGroup != null)
        {
            objectGroup.DeadEnemy++;
        }
        if (enemyState.TargetFriend != null)
        {
            if (enemyState.TargetFriend.GetComponent<Friend>() != null)
            {
                enemyState.TargetFriend.GetComponent<Friend>().Dance();
            }
        }
        ActivateOnDeathObjects();
    }

    public void StenaCollision(GameObject Stn)
    {
        StopMove();
        ChangeColor();
        follow = false;
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        Invoke(nameof(EnabledCheck), 4f);

        //Invoke(nameof(DisabledRagdoll), 0.1f);
        if (bulletTouchMe)
        {
            if (bullet != null)
            {
                bullet.GetComponent<Bullet>().StenaColl(Stn);
                bullet.GetComponent<Bullet>().needMoveForward = false;
            }
        }
        if (enemyState.TargetFriend != null)//включаем танец
        {
            if(enemyState.TargetFriend.TryGetComponent(out Friend friend))
            {
                friend.Dance();
            }
        }
        enemyShoot.StopAllCoroutines();
    }

    private void SelfUnhook()
    {
        if (IsDead) return;

        LineMainBodyWithRagdoll();
        UnhookBodyPartConfigurableJoints();
        DisableRagdoll();
        enemyAnimator.Enable();
        EnableControl();
    }

    public void StopMove()
    {
        gameObject.GetComponent<Move>().SpeedMove = 0f;     
    }
    public void ShootMe()
    {
        bulletTouchMe = true;
        StopMove();
    }
    public void EnableRagdoll()
    {
        
        for (int i = 0; i < RbChild.Count; i++)
        {
            RbChild[i].isKinematic = false;
        }
        DisableControll();
    }
    public void BlockPelvis()
    {
        Invoke(nameof(LockPelvis), 0.15f);
        RbPelvis.useGravity = false;
        RbPelvis.mass = 1f;
        Invoke(nameof(DisblockPelvis), 1f);
    }
    private void DisableControll()
    {
        enemyState.StopMovement();
        enemyAnimator.Disable();
        gameObject.GetComponent<Move>().enabled = false;
    }
    private void EnableControl()
    {
        if (objectGroup != null)
        {
            enemyState.StartAttack();
        }
        else
        {
            enemyState.MoveToPoint(startPoint);
        }
        enemyKillTimer.Stop();
        gameObject.GetComponent<Move>().enabled = true;
    }
    public void DisableRagdoll()
    {
        for (int i = 0; i < RbChild.Count; i++)
        {
            RbChild[i].isKinematic = true;
        }
    }
    void DisabledGravity()
    {
        for (int i = 0; i < RbChild.Count; i++)
        {
            RbChild[i].useGravity = false;
        }
    }
    void ChangeColor()
    {
        Material[] sharedMaterialsCopy = meshRenderer.sharedMaterials;
        for (int i = 0; i < sharedMaterialsCopy.Length; i++)
        {
            sharedMaterialsCopy[i] = GrayMaterial;
            meshRenderer.sharedMaterials = sharedMaterialsCopy;
        }
    }
    void EnabledCheck()
    {
        check.Complette();
    }
    void LockPelvis()
    {
        RbPelvis.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ |
        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
    void DisblockPelvis()
    {
        RbPelvis.constraints = RigidbodyConstraints.None;
        
    }
    void BlockPelvisY()
    {
        RbPelvis.constraints =
        RigidbodyConstraints.FreezePositionY;
    }

    public void SetPelvisKinematic(bool enabled)
    {
        RbPelvis.isKinematic = enabled;
    }

    public void SetPelvisPosition(Vector3 position)
    {
        RbPelvis.transform.position = position;
    }

    public bool IsHooked()
    {
        bool isHooked = false;
        foreach (BodyPart bodyPart in bodyParts)
        {
            if (bodyPart.IsHooked)
            {
                isHooked = true;
                break;
            }
        }

        return isHooked;
    }

    public Transform GetHookedTransform()
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            if (bodyPart.IsHooked)
            {
                return bodyPart.transform;
            }
        }
        return null;
    }

    public void UnhookBodyPartSpringJoints()
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.UnhookSpringJoint();
        }
    }

    public void UnhookBodyPartConfigurableJoints()
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            if (bodyPart.IsHooked)
                bodyPart.UnhookConfigurableJoint();
        }
    }

    public void RehookBodyPartFixedJoints()
    {
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.RehookSpringJoint();
        }
    }

    public void ResetRigidbodiesVelocities()
    {
        for (int i = 0; i < RbChild.Count; i++)
        {
            RbChild[i].velocity = Vector3.zero;
        }
    }

    public Vector3 GetFarthestPoint(Vector3 fromPosition, Vector3 direction)
    {
        float maxDistance = float.NegativeInfinity;
        Vector3 maxPoint = Vector3.zero;
        foreach (Collider collider in _collidersForFindingFarthestPoint)
        {
            Vector3 closestPoint = collider.ClosestPointOnBounds(fromPosition + direction * 20f);
            float distance = Vector3.Distance(fromPosition, closestPoint);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxPoint = closestPoint;
            }
            Debug.DrawLine(closestPoint, closestPoint + Vector3.up, Color.green, 5f);
        }
        Debug.DrawLine(maxPoint, maxPoint + Vector3.up, Color.blue, 5f);

        maxPoint += direction * 0.2f;

        return maxPoint;
    }

    public void SetFixedHeadBone(bool isFixed)
    {
        if (headRigidbody == null)
        {
            throw new System.Exception("You didn't link headRigidbody object.");
        }
        if (headJointObject == null)
        {
            throw new System.Exception("headJointObject didn't created.");
        }

        if (isFixed)
        {
            SpringJoint springJoint = headJointObject.AddComponent<SpringJoint>();
            springJoint.connectedBody = headRigidbody;
            springJoint.spring = 7000f;
            springHeadObject.SetSpringJoint(springJoint);

            if (headJointObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
            }
        }
        else
        {
            if (headJointObject.TryGetComponent(out SpringJoint springJointComponent))
            {
                Destroy(springJointComponent);
            }
        }
    }
    public void FaceAtack() //CallbakFromEnemyState
    {
        for (int i = 0; i < Eyes.Length; i++)
        {
            Eyes[i].SetActive(false);
        }
        Eyes[0].SetActive(true);
        for (int w = 0; w < Mounts.Length; w++)
        {
            Mounts[w].SetActive(false);
        }
        Mounts[0].SetActive(true);
    }
    private void FaceBulletCollision()
    {
        for (int i = 0; i < Eyes.Length; i++)
        {
            Eyes[i].SetActive(false);
        }
        Eyes[1].SetActive(true);
        for (int w = 0; w < Mounts.Length; w++)
        {
            Mounts[w].SetActive(false);
        }
        Mounts[1].SetActive(true);
    }
    private void FaceDeath()
    {
        for (int i = 0; i < Eyes.Length; i++)
        {
            Eyes[i].SetActive(false);
        }
        Eyes[2].SetActive(true);
        for (int w = 0; w < Mounts.Length; w++)
        {
            Mounts[w].SetActive(false);
        }
        Mounts[1].SetActive(true);
    }

    private void CreateHeadJoint()
    {
        headJointObject = new GameObject("HeadJoint");
        headJointObject.transform.position = headRigidbody.gameObject.transform.position;
        headJointObject.transform.parent = transform;
        springHeadObject = headJointObject.AddComponent<SpringHeadObject>();
        springHeadObject.SetTarget(RbPelvis.transform);
    }

    public void CheckAndActivateWebConditions()
    {
        foreach (EnemyWebActivateCondition enemyWebActivateCondition in enemyWebActivateConditions)
        {
            if (enemyWebActivateCondition.Check())
            {
                enemyWebActivateCondition.Activate();
            }
        }
    }

    public void AddVelocityToAllBodyParts(Vector3 velocity, float multiplier = 1f)
    {
        foreach (Rigidbody rb in RbChild)
        {
            //rb.velocity = velocity * multiplier;
            rb.AddForce(velocity * multiplier, ForceMode.VelocityChange);
        }
    }

    public void LineMainBodyWithRagdoll()
    {
        float y = transform.position.y;

        Vector3 newPosition = RbPelvis.transform.position;
        newPosition.y = y;
        transform.position = newPosition;
    }
}
