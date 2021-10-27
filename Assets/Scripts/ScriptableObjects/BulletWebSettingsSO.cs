using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands.ScriptableObjects
{
    [CreateAssetMenu(fileName = "BulletWebSettings", menuName = "ScriptableObjects/BulletWeb/Create settings")]
    public class BulletWebSettingsSO : ScriptableObject
    {
        [SerializeField] private float speed = 30f;
        public float Speed { get => speed; }

        [SerializeField] private bool canCollideWithHookedParts = true;
        public bool CanCollideWithHookedParts { get => canCollideWithHookedParts; }

        [Space]

        [SerializeField] private Vector2 startRandomScale = new Vector2(0.01f, 0.1f);
        public Vector2 StartRandomScale { get => startRandomScale; }

        [SerializeField] private Vector2 endRandomScale = new Vector2(0.6f, 1f);
        public Vector2 EndRandomScale { get => endRandomScale; }

        [SerializeField] private Vector2 stickedToWallRandomScale = new Vector2(0.6f, 1f);
        public Vector2 StickedToWallRandomScale { get => stickedToWallRandomScale; }

        [Space]

        [SerializeField] private float changeScaleTime = 0.2f;
        public float ChangeScaleTime { get => changeScaleTime; }

        [SerializeField] private float timeToResetTipPoint = 0.5f;
        public float TimeToResetTipPoint { get => timeToResetTipPoint; }

        [SerializeField] private float maximumTipLength = 0.07f;
        public float MaximumTipLength { get => maximumTipLength; }

        [SerializeField] private float groundCatchTipLength = 0.03f;
        public float GroundCatchTipLength { get => groundCatchTipLength; }
    }
}