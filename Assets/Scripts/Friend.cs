using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
public class Friend : MonoBehaviour, IHittableByEnemy, IHookable, IDeadable, IWebable, IWebConditionChecked
{
    public Material GrayMaterial;
    public SkinnedMeshRenderer meshRenderer;
    [SerializeField] private Animator animator;
    private Rigidbody[] childRigidbodies;
    [SerializeField] private Collider[] _collidersForFindingFarthestPoint;

    [SerializeField] private int _shotsToDeath = 1;
    private int _currentTakenShots = 0;

    [Space]
    [Header("Death")]
    [SerializeField] private GameObject[] AcivateObjectsOnDeath;

    private bool _isDead = false;
    public bool IsDead => _isDead;

    private BodyPart[] bodyParts;

    private void Awake()
    {
        bodyParts = GetComponentsInChildren<BodyPart>();
        foreach (BodyPart bodyPart in bodyParts)
        {
            bodyPart.Init(this, this, this, this);
        }

        childRigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    public void TakeDamage()
    {
        if (_isDead) return;

        _currentTakenShots++;

        if (_currentTakenShots >= _shotsToDeath)
        {
            ChangeColor();
            _isDead = true;
            ActivateOnDeathObjects();
        }

        animator.enabled = false;
        EnableRagdoll();
        Invoke("DisableGravity", 2.5f);
        GameManager.Instance.Lose();
        
    }
    public void EnableRagdoll()
    {
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            childRigidbodies[i].isKinematic = false;
        }
    }
    void DisableRagdoll()
    {
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            childRigidbodies[i].isKinematic = true;
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
    public void Pray()
    {
        if (animator != null)//////////////
        {
            animator.SetTrigger("pray");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BulletEnemy"))
        {
            TakeDamage();
            Destroy(other.gameObject);
            Debug.Log("в меня попал враг");
        }

        if (other.gameObject.CompareTag("FallingObject"))
        {
            TakeDamage();
        }
    }
    public void Dance()
    {
        animator.SetTrigger("dance");
    }
    public void Fear()
    {
        animator.SetTrigger("backRun");
    }
    public void EnemyHit()
    {
        FixedJoint fixedJoint = childRigidbodies[0].gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            childRigidbodies[i].AddForce(-transform.forward * 100);
           
        }

    }
    void DisableGravity()
    {
        for (int i = 0; i < childRigidbodies.Length; i++)
        {
            childRigidbodies[i].mass = 10f;
            childRigidbodies[i].angularDrag = 10f;
            if (i > 1)
            {
                childRigidbodies[i].gameObject.GetComponent<CharacterJoint>().enableCollision = false;
            }
        }
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Hit(GameObject bulletObject, float damage = 1f)
    {
        animator.enabled = false;
        TakeDamage();
    }

    public void SetFixedHeadBone(bool enabled)
    {

    }

    public bool IsHooked()
    {
        return false;
    }

    public void ResetRigidbodiesVelocities()
    {

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

    public Transform GetHookedTransform()
    {
        return null;
    }

    public void UnhookBodyPartSpringJoints()
    {
        Debug.LogError("UnhookBodyPartSpringJoints: NotImplementedException");
    }

    public void CheckAndActivateWebConditions()
    {
        Debug.LogError("CheckAndActivateWebConditions: NotImplementedException");
    }

    public void AddVelocityToAllBodyParts(Vector3 velocity, float multiplier = 1)
    {
        Debug.LogError("AddVelocityToAllBodyParts: NotImplementedException");
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
        throw new System.NotImplementedException();
    }
}
