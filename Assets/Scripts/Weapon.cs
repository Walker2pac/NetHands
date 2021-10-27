using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stena"))
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.SetParent(collision.gameObject.transform);

        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            transform.SetParent(null);
            collision.gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().mass = 5f;
            CharacterJoint characterJoint = collision.gameObject.AddComponent<CharacterJoint>();
            characterJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            transform.SetParent(null);
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<Rigidbody>().mass = 5f;
            CharacterJoint characterJoint = other.gameObject.AddComponent<CharacterJoint>();
            characterJoint.connectedBody = gameObject.GetComponent<Rigidbody>();
        }
    }
}
