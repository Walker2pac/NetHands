using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ObjectFalling : MonoBehaviour, IWebable, IActivateable
{
    public Material SilverMaterial;
    public MeshRenderer meshRenderer;
    public Check check;
    private Rigidbody _rb;
    public float TimeToFalling;
    [SerializeField] private float shakingTime = 2f;
    private FixedJoint  fixedJoint;

    private Collider _collider;
    void Awake()
    {
        if (shakingTime <= 0f) shakingTime = 0.1f;

        _rb = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();
    }

    public void Activate()
    {
        StartCoroutine(Falling());
    }

    IEnumerator Falling()
    {
        yield return Shaking(TimeToFalling);

        _rb.isKinematic = false;
        Invoke(nameof(Comp), 4f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stena"))
        {
            StopAllCoroutines();
            check.Complette();
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            if (fixedJoint!= null)
            {
                Destroy(fixedJoint);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            meshRenderer.material = SilverMaterial;
            StopAllCoroutines();
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().mass = 10f;

            fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.enablePreprocessing = false;
            fixedJoint.connectedBody = other.gameObject.GetComponent<Rigidbody>();
            check.Complette();
        }
    }
    private void Comp()
    {
        check.Complette();
    }

    public Vector3 GetFarthestPoint(Vector3 fromPosition, Vector3 direction)
    {
        Vector3 maxPoint = Vector3.zero;
        if (_collider != null)
        {
            maxPoint = _collider.ClosestPointOnBounds(fromPosition + direction * 20f);
        }
        return maxPoint;
    }

    private IEnumerator Shaking(float time)
    {
        float waitTime = time - shakingTime;
        if (waitTime < 0f) waitTime = 0f;

        yield return new WaitForSeconds(waitTime);

        Vector3 startPosition = transform.position;
        Vector3 modifiedPosition = transform.position;
        float speed;
        float currentAmount;
        float amount = 0.05f;
        float currentTime = 0f;

        while (currentTime <= shakingTime)
        {
            speed = 10f * (currentTime / shakingTime);
            currentAmount = amount * (currentTime / shakingTime);
            modifiedPosition.y += Mathf.Sin(Time.time * speed) * currentAmount;
            transform.position = modifiedPosition;
            //Debug.Log($"{gameObject.name} - {currentAmount}");
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.position = startPosition;
    }
}
