using NetHands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NetHandsButton
{
    public class Button : MonoBehaviour, IBulletInteractable
    {
        public MeshRenderer meshRenderer;
        public Material ChangedMaterial;
        public UnityEvent ButtonAction;

        private bool _isInteractable = true;

        public bool IsInteractable => _isInteractable;

        public Transform GetTransform()
        {
            return transform;
        }

        public void Interact()
        {
            _isInteractable = false;

            gameObject.GetComponent<Check>().ObjectIsComplette = true;
            ButtonAction.Invoke();
            meshRenderer.material = ChangedMaterial;
            Debug.Log("Button interacted");
            gameObject.GetComponent<Animator>().enabled = true;
        }
    }
}

