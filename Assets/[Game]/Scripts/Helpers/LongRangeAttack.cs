using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UOP1.Helper
{
    public class LongRangeAttack : MonoBehaviour
    {

        public GameObject parentUnit;

        public void TriggerFireSpawnEvent()
        {
            LevelManager.Instance.AnimationEventTriggered(parentUnit);
        }


    }
}

