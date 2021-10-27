using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittableByEnemy
{
    void TakeDamage();
    void EnemyHit();
}
