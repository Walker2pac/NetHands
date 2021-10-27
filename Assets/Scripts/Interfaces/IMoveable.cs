using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    bool IsMoving { get; }
    IEnumerator Move();
    Transform GetTransform();
}
