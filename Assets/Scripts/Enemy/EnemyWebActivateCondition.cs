using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetHands.Enemy
{
    [System.Serializable]
    public class EnemyWebActivateCondition
    {
        [SerializeField] private string name;
        [SerializeField] private BodyPart[] conditionParts;
        [SerializeField] private GameObject[] toActivate;

        public bool Check()
        {
            bool isConfirmed = false;

            foreach (BodyPart conditionPart in conditionParts)
            {
                isConfirmed = conditionPart.IsHooked || conditionPart.IsSpringHooked;

                if (!isConfirmed) break;
            }

            return isConfirmed;
        }

        public void Activate()
        {
            foreach (GameObject activateGameObject in toActivate)
            {
                activateGameObject.SetActive(true);
            }
        }
    }
}