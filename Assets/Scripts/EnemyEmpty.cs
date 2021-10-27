using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEmpty : MonoBehaviour
{
    public Enemy ParentScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stena"))
        {
            ParentScript.StenaCollision(other.gameObject);
        }
    }
}
