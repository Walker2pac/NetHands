using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BulletEmpty : MonoBehaviour
{
    public Bullet bullet;
    private BoxCollider collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stena"))
        {
            bullet.StenaColl(other.gameObject);
            collider.enabled = false;
            if (transform.root.gameObject.GetComponent<Rigidbody>() != null)
            {
                transform.root.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
          
        }
    }
    
}
