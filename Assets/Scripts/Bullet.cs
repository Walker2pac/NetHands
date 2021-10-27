using NetHands;
using NetHands.Gameplay.Shooting;
using NetHands.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletWebSettingsSO _settings;
    [Space]
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private GameObject _bulletModel;
    [SerializeField] private ParticleSystem _particlesOnStickingToWall;
    private Transform _limitObject;
    private Vector2 _limitMinMax = new Vector2(2f, 2.6f);

    public BoxCollider boxCollider;
    private Rigidbody rb;
    private int collisionAmount;
    private Vector3 StartPos, CurrentPos, Direction;
    public GameObject Empty;
    public GameObject Test;
    public bool needMoveForward;
    private bool StenaTrig;
    private CharacterJoint characterJoint;

    [SerializeField] private bool _moveBulletToWallwebTipPoint = false;
    [SerializeField] private bool _enableGravityOnHitHookedTarget = true;
    [SerializeField] private bool _disableBulletOnHookedTargetHit = false;
    [SerializeField] private WallWeb wallWebPrefab;
    private WallWeb wallWeb;
    public WallWeb WallWeb => wallWeb;

    private IHookable hookable;
    private IDeadable deadable;
    public IDeadable Deadable => deadable;
    private IWebable webable;

    void Start()
    {
        SetEnabledModel(true);

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        needMoveForward = true;

        StartPos = transform.position;

        CreateWallWeb();
        StartCoroutine(CheckDistance());
    }

    void FixedUpdate()
    {
        if (needMoveForward)
        {
            rb.MovePosition(transform.position + Direction * _settings.Speed * Time.fixedDeltaTime);
        }
    }

    private void LateUpdate()
    {
        if (_limitObject != null)
        {
            float sqrMagnitude = (_limitObject.position - transform.position).magnitude;
            if (sqrMagnitude > _limitMinMax.y)
            {
                transform.position = _limitObject.position + (transform.position - _limitObject.position).normalized * _limitMinMax.y;
            }
            if (sqrMagnitude < _limitMinMax.x)
            {
                transform.position = _limitObject.position + (transform.position - _limitObject.position).normalized * _limitMinMax.x;
            }
        }
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void SetLimitTranform(Transform limitObject, float distanceBetween)
    {
        //_limitMinMax = new Vector2(Mathf.Clamp(distanceBetween - 0.4f, 0f, distanceBetween), Mathf.Clamp(distanceBetween - 0.1f, 0f, distanceBetween));
        _limitMinMax = new Vector2(distanceBetween - 0.01f, distanceBetween);
        _limitObject = limitObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            BodyPart bodyPart = null;
            if (hookable == null)
            {
                if (other.gameObject.TryGetComponent(out BodyPart bodyPartComponent))
                {
                    hookable = bodyPartComponent.Hookable;
                    deadable = bodyPartComponent.Deadable;
                    bodyPart = bodyPartComponent;
                }
                else if (other.gameObject.TryGetComponent(out Enemy enemyComp))
                {
                    hookable = enemyComp;
                }
            }

            if (deadable.IsDead)
            {
                hookable = null;
                return;
            }

            collisionAmount++;
            if (collisionAmount < 2)
            {
                if (!_settings.CanCollideWithHookedParts && (hookable.IsHooked() && bodyPart != null && (bodyPart.IsHooked || bodyPart.IsSpringHooked)))
                {
                    Debug.Log("Trying to hook already hooked bodyPart");
                    return;
                }

                OnEnemyCollision();

                CurrentPos = transform.position;

                if (_enableGravityOnHitHookedTarget && hookable.IsHooked())
                {
                    needMoveForward = false;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(transform.forward * 6000f, ForceMode.Impulse);
                }

                if (_disableBulletOnHookedTargetHit && hookable.IsHooked())
                {
                    Destroy(gameObject);
                    gameObject.SetActive(false);
                }

                if (bodyPart != null)
                {
                    bodyPart.TriggerCollide(boxCollider);
                    boxCollider.enabled = false;
                }
            }
            //needMoveForward = true;

            if (_particlesOnStickingToWall != null)
            {
                _particlesOnStickingToWall.Play();
            }
        }
        if (other.gameObject.CompareTag("Friend"))
        {
            BodyPart bodyPart = null;
            if (hookable == null)
            {
                if (other.gameObject.TryGetComponent(out BodyPart bodyPartComponent))
                {
                    Debug.Log($"Bullet collided with {bodyPartComponent.gameObject.name}");
                    hookable = bodyPartComponent.Hookable;
                    deadable = bodyPartComponent.Deadable;
                    bodyPart = bodyPartComponent;
                }
            }

            if (deadable != null && deadable.IsDead)
            {
                hookable = null;
                return;
            }

            if (hookable == null) return;

            collisionAmount++;
            if (collisionAmount < 2)
            {
                OnFriendCollision();

                CurrentPos = transform.position;

                if (_enableGravityOnHitHookedTarget && hookable.IsHooked())
                {
                    needMoveForward = false;
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.velocity = Vector3.zero;
                    rb.AddForce(transform.forward * 6000f, ForceMode.Impulse);
                }

                if (bodyPart != null)
                {
                    bodyPart.TriggerCollide(boxCollider);
                    boxCollider.enabled = false;
                }
            }

        }

        if (other.gameObject.TryGetComponent(out IWebable webable))
        {
            this.webable = webable;
        }

        if (other.gameObject.TryGetComponent(out IBulletInteractable interactable))
        {
            interactable.Interact();
        }
    }

    public virtual void OnEnemyCollision() { }
    public virtual void OnFriendCollision() { }

    Vector3  FindDirection()
    {
        Vector3 Dir = CurrentPos - StartPos;
        Dir.Normalize();
        return Dir;
    }

    private Vector3 GetToStartDirection()
    {
        Vector3 direction = StartPos - CurrentPos;
        return direction.normalized;
    }
    private float Distane()
    {
        float Dis = Vector3.Distance(StartPos, transform.position);
        return Dis;
    }

    public void StenaColl(GameObject Stena)
    {
        StenaTrig = true;

        rb.isKinematic = true;
        rb.useGravity = false;

        SetEnabledModel(false);


        if (Stena.TryGetComponent(out Collider collider))
        {
            Vector3 closestPoint = collider.ClosestPoint(transform.position);

            if (closestPoint == transform.position)
            {
                Debug.Log("Bullet position is the same with closestPoint");
            }

            Vector3 direction = (transform.position - closestPoint).normalized;
            if (direction == Vector3.zero)
            {
                direction = -Direction;
                //direction = -transform.forward;
            }

            wallWeb.ResetScale();

            wallWeb.transform.position = closestPoint;
            wallWeb.transform.rotation = Quaternion.identity;

            wallWeb.transform.SetParent(Stena.transform, true);
            gameObject.transform.SetParent(Stena.transform, true);

            wallWeb.ModelTransform.rotation = Quaternion.LookRotation(direction);


            wallWeb.ModelTransform.RotateAround(wallWeb.ModelTransform.position, wallWeb.ModelTransform.right, 90);

            float dot = Vector3.Dot(wallWeb.ModelTransform.up, Vector3.up);

            if (webable != null)
            {
                if (dot < 0.9f)
                {
                    wallWeb.SetTipPointPosition(webable.GetFarthestPoint(Stena.transform.position, wallWeb.ModelTransform.up));
                }
                else
                {
                    wallWeb.SetTipPointOnGroundCatchPosition();
                }
            }

            if (hookable != null)
            {
                hookable.ResetRigidbodiesVelocities();
            }

            wallWeb.PlayShakingAnimation();
        }

        if (_moveBulletToWallwebTipPoint)
        {
            transform.position = wallWeb.GetTipPointTransform().position;
        }
        needMoveForward = false;
        boxCollider.enabled = false;

        _trailRenderer.enabled = false;
    }

    private void CreateWallWeb()
    {
        wallWeb = Instantiate(wallWebPrefab, transform.position, transform.rotation);
        wallWeb.transform.SetParent(transform, true);
        wallWeb.SetRandomScale();
        wallWeb.SetShootScale();
    }

    public void ResetWallWebTipPoint()
    {
        if (wallWeb != null)
        {
            wallWeb.ResetTipPoint();
        }
    }

    IEnumerator Testt()
    {
        Quaternion target = Quaternion.Euler(60f, 0f, 0f);
        Vector3 Pos = transform.position;
        Pos.y -= 1f;
        while (transform.rotation != target)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, target, 4 * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, Pos, 1 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator CheckDistance()
    {
        float time = 2f;
        for (; ; )
        {
            float dis = Distane();//проверяем дистанцию
            if(dis > 120f)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(time);
        }
    }


    private void SetEnabledModel(bool enabled)
    {
        if (_bulletModel == null) return;

        _bulletModel.SetActive(enabled);
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
}
