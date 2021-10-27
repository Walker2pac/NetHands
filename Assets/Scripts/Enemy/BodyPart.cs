using NetHands.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BodyPart : MonoBehaviour, IWebable
{
    private IHookable _hookable;
    public IHookable Hookable => _hookable;

    private IDeadable _deadable;
    public IDeadable Deadable => _deadable;

    private IWebable _webable;
    public IWebable Webable => _webable;

    private IWebConditionChecked _webConditionChecked;
    public IWebConditionChecked WebConditionChecked => _webConditionChecked;

    [SerializeField] private bool isFixingHeadOnHit = false;
    [SerializeField] private GameObject[] gameObjectsToActivateOnHit;
    [SerializeField] private float damageOnHit = 1f;
    [SerializeField] private bool freeRotationOnConnection = false;
    [SerializeField] private bool rotateToConfigurableUpDirection = false;

    private SpringJoint springJoint;
    private ConfigurableJoint configurableJoint;
    private Rigidbody previousConnectedRigibody;

    [Space]
    [Header("Second hit")]
    [SerializeField] private bool _isWebSiblingsOnSecondHit = false;
    [SerializeField] private GameObject[] _siblingsGameObjectsToActivateOnSecondHit;
    private bool _isPreviouslyHit = false;

    private Rigidbody rb;
    private Collider collider;

    public bool IsHooked => configurableJoint != null;
    public bool IsSpringHooked => springJoint != null;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError($"No collider in BodyPart {gameObject.GetFullName()}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stena"))
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void Init(IHookable hookable, IDeadable deadable, IWebable webable, IWebConditionChecked webConditionChecked) 
    {
        _hookable = hookable;
        _deadable = deadable;
        _webable  = webable;
        _webConditionChecked = webConditionChecked;
    }

    public void TriggerCollide(Collider bulletCollider)
    {
        if (_deadable.IsDead) return;

        _hookable.Hit(bulletCollider.gameObject, damageOnHit);
        _hookable.SetFixedHeadBone(false);

        _hookable.EnableRagdoll();

        if (!_hookable.IsHooked())
        {
            CreateConfigurableJoint(bulletCollider.gameObject);
            if (bulletCollider.gameObject.TryGetComponent(out Bullet bullet))
            {
                Vector3 bulletVelocity = bullet.GetVelocity();
                _hookable.ResetRigidbodiesVelocities();
               // _hookable.AddVelocityToAllBodyParts(bulletVelocity, 1f);
            }

            if (transform.childCount > 0)
            {
                SetChildPositionAnchorToConfigurableJoint();
            }
        }
        else
        {
            //_hookable.UnhookBodyPartSpringJoints();
            //CreateSpringJoint(bulletCollider.gameObject);
        }

        if (isFixingHeadOnHit && !_deadable.IsDead)
        {
            _hookable.SetFixedHeadBone(true);
        }

        ActivateHitObjects();
        _webConditionChecked.CheckAndActivateWebConditions();

        if (_isWebSiblingsOnSecondHit && _isPreviouslyHit)
        {
            ActivateSiblingsHitObjects();
        }

        _isPreviouslyHit = true;
    }

    private void CreateConfigurableJoint(GameObject bullet)
    {
        if (rotateToConfigurableUpDirection)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        }

        configurableJoint = gameObject.AddComponent<ConfigurableJoint>();
        configurableJoint.connectedBody = bullet.GetComponent<Rigidbody>();

        configurableJoint.autoConfigureConnectedAnchor = false;
        configurableJoint.connectedAnchor = Vector3.zero;
        configurableJoint.enablePreprocessing = false;

        configurableJoint.xMotion = ConfigurableJointMotion.Locked;
        configurableJoint.yMotion = ConfigurableJointMotion.Locked;
        configurableJoint.zMotion = ConfigurableJointMotion.Locked;

        if (freeRotationOnConnection)
        {
            configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
            configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
            configurableJoint.angularZMotion = ConfigurableJointMotion.Free;
        }
        else
        {
            configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
            configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
            configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
        }

        configurableJoint.axis = new Vector3(0f, 0f, 1f);
        configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;

        SoftJointLimitSpring softJointLimitSpring = new SoftJointLimitSpring();
        softJointLimitSpring.spring = 250f;
        softJointLimitSpring.damper = 250f;
        SoftJointLimit softJointLimit = new SoftJointLimit();
        softJointLimit.limit = -30f;
        SoftJointLimit highSoftJointLimit = new SoftJointLimit();
        highSoftJointLimit.limit = -30f;

        configurableJoint.linearLimitSpring = softJointLimitSpring;
        configurableJoint.lowAngularXLimit = softJointLimit;
        configurableJoint.highAngularXLimit = highSoftJointLimit;
    }

    private void CreateSpringJoint(GameObject bullet)
    {
        springJoint = gameObject.AddComponent<SpringJoint>();
        springJoint.enablePreprocessing = false;
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = Vector3.zero;
        springJoint.spring = 1000000f;
        springJoint.damper = 1f;
        springJoint.tolerance = 0.001f;
        springJoint.connectedBody = bullet.GetComponent<Rigidbody>();
        springJoint.massScale = 10000f;
        springJoint.maxDistance = 0.1f;
        springJoint.connectedMassScale = 10f;

        if (bullet.TryGetComponent(out Bullet bulletComponent))
        {
            Transform hookedTransform = _hookable.GetHookedTransform();
            if (hookedTransform != null)
            {
                float distanceBetweenHookedAndCurrent = Vector3.Distance(hookedTransform.position, transform.position);
                bulletComponent.SetLimitTranform(hookedTransform, distanceBetweenHookedAndCurrent);
            }
        }
    }

    private void SetChildPositionAnchorToConfigurableJoint()
    {
        Transform childTransform = transform.GetChild(0);
        Vector3 anchorVector3 = transform.InverseTransformPoint(childTransform.position);
        anchorVector3.x = 0f;
        anchorVector3.z = 0f;
        configurableJoint.anchor = anchorVector3;
    }

    public void UnhookConfigurableJoint()
    {
        if (configurableJoint != null)
        {
            if (configurableJoint.connectedBody.gameObject.TryGetComponent(out Bullet bullet))
            {
                bullet.ResetWallWebTipPoint();
            }

            Destroy(configurableJoint);
            configurableJoint = null;
        }
    }

    public void UnhookSpringJoint()
    {
        if (springJoint != null)
        {
            if (springJoint.connectedBody.gameObject.TryGetComponent(out Bullet bullet))
            {
                bullet.ResetWallWebTipPoint();
            }

            previousConnectedRigibody = springJoint.connectedBody;

            Destroy(springJoint);
            springJoint = null;
        }
        else
        {
            previousConnectedRigibody = null;
        }
    }

    public void RehookSpringJoint()
    {
        if (previousConnectedRigibody != null)
        {
            springJoint = gameObject.AddComponent<SpringJoint>();
            springJoint.enablePreprocessing = false;
            springJoint.connectedBody = previousConnectedRigibody;
        }
    }

    public Vector3 GetFarthestPoint(Vector3 fromPosition, Vector3 direction)
    {
        return _webable.GetFarthestPoint(fromPosition, direction);
    }

    private void ActivateHitObjects()
    {
        foreach (GameObject gameObjectActivateOnHit in gameObjectsToActivateOnHit)
        {
            gameObjectActivateOnHit.SetActive(true);
        }
    }

    private void ActivateSiblingsHitObjects()
    {
        foreach (GameObject _siblingsGameObjectToActivateOnSecondHit in _siblingsGameObjectsToActivateOnSecondHit)
        {
            _siblingsGameObjectToActivateOnSecondHit.SetActive(true);
        }
    }
}
