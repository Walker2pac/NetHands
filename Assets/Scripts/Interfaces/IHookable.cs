using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHookable
{
    void Hit(GameObject bulletObject, float damage = 1f);
    void EnableRagdoll();
    void SetFixedHeadBone(bool enabled);
    bool IsHooked();
    void ResetRigidbodiesVelocities();
    void UnhookBodyPartSpringJoints();
    void AddVelocityToAllBodyParts(Vector3 velocity, float multiplier = 1f);
    Transform GetHookedTransform();
}
