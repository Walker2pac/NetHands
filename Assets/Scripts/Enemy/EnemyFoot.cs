using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFoot : MonoBehaviour
{
    public GameObject[] Foots = new GameObject[0];



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            DisableCollider();
        }
        if (collision.gameObject.CompareTag("Stena"))
        {
            DisableCollider();
        }
    }
    public void DisableCollider()
    {
        for (int i = 0; i < Foots.Length; i++)
        {
            Foots[i].GetComponent<CapsuleCollider>().enabled = false;

        }
    }
}
