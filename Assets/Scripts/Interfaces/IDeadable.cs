using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeadable
{
    bool IsDead { get; }
    void Kill();
}
