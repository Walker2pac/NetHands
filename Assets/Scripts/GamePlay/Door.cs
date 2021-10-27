using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands.Gameplay
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Vector3 axisRotation = new Vector3(0f, 1f, 0f);
        [SerializeField] private float openedRotation = 0f;
        [SerializeField] private float closedRotation = 0f;
        [SerializeField] private float time;

        private IEnumerator movingCoroutine;

        public void Open()
        {
            StopMoving();
            movingCoroutine = Move(openedRotation);
            StartCoroutine(movingCoroutine);
        }

        public void Close()
        {
            StopMoving();
            movingCoroutine = Move(closedRotation);
            StartCoroutine(movingCoroutine);
        }

        private IEnumerator Move(float to)
        {
            Quaternion rotation = Quaternion.Euler(axisRotation.normalized * to);
            Quaternion newRotation = rotation;
            Quaternion startRotation = transform.rotation;
            float currentTime = 0f;

            while (currentTime <= time)
            {
                transform.rotation = Quaternion.Slerp(startRotation, newRotation, currentTime / time);

                currentTime += Time.deltaTime;
                yield return null;
            }

            yield return null;
        }

        private void StopMoving()
        {
            if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        }
    }
}