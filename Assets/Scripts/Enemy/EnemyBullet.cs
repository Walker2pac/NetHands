using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.CompareTag("Stena"))
        {
            Destroy(gameObject);
        }
       
        if (collision.gameObject.CompareTag("Friend"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stena"))
        {
            Destroy(gameObject);
        }
      
        if (other.gameObject.CompareTag("Friend"))
        {
            Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
