using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands
{
    public interface IBulletInteractable
    {
        bool IsInteractable { get; }
        void Interact();
        Transform GetTransform();
    }
}