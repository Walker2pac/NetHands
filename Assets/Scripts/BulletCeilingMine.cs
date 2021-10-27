using NetHands.Gameplay.Shooting;
using NetHands.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCeilingMine : Bullet
{
    public override void OnEnemyCollision()
    {
        SetDirection(Vector3.up);
    }

    public override void OnFriendCollision()
    {
        SetDirection(Vector3.up);
    }
}
