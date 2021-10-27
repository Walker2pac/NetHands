using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMove : MonoBehaviour, IMoveable
{
    public Transform EndPos;
    public float Speed;
    private Bullet[] bullets;
    private bool isMoving = false;

    public bool IsMoving => isMoving;

    private void OnEnable()
    {
        bullets = GetComponentsInChildren<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            if (bullet.Deadable != null && !bullet.Deadable.IsDead)
            {
                bullet.Deadable.Kill();
            }
        }
    }

    void Start()
    {
        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        isMoving = true;
        Vector3 newPosition = EndPos.position;
        while (transform.position != newPosition)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Speed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
