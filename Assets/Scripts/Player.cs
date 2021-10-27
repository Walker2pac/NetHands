using Dreamteck.Splines;
using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IHittableByEnemy
{
    public void EnemyHit()
    {
        GameState.Instance.ChangeState(GameState.State.Lose);
    }

    public void TakeDamage() { }
}
