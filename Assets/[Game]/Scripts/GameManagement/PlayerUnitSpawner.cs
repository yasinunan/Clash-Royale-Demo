using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

namespace UOP1.Helper
{
    public class PlayerUnitSpawner : Singleton<PlayerUnitSpawner>
    {

        private const string PLAYER_GROUND_UNIT = "PlayerGroundUnit";
        private const string PLAYER_AERIAL_UNIT = "PlayerAerialUnit";
        private const string PLAYER_LONGRANGE_UNIT = "PlayerLongRangeUnit";
         private const string PLAYER_SUPPORT_UNIT = "PlayerSupportUnit";


        [SerializeField] Transform unitHolderTransform;
        public List<GameObject> groundedPlayerList;
        public List<GameObject> aerialPlayerList;

        //===============================================================================================

        public void SpawnGroundUnit(Vector3 position)
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(PLAYER_GROUND_UNIT);

            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = position;
                groundedPlayerList.Add(go);
            }
            else
            {
                Debug.Log("Could not  spawn player ground unit");
            }
        }

        //===============================================================================================

         public void SpawnLongRangeUnit(Vector3 position)
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(PLAYER_LONGRANGE_UNIT);

            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = position;
                groundedPlayerList.Add(go);
            }
            else
            {
                Debug.Log("Could not  spawn player long range unit");
            }
        }

        //===============================================================================================

         public void SpawnAerialUnit(Vector3 position)
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(PLAYER_AERIAL_UNIT);

            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = position;
                aerialPlayerList.Add(go);
            }
            else
            {
                Debug.Log("Could not  spawn player aerial unit");
            }
        }
          //===============================================================================================

         public void SpawnSupportUnit(Vector3 position)
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(PLAYER_SUPPORT_UNIT);

            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = position;
                groundedPlayerList.Add(go);
            }
            else
            {
                Debug.Log("Could not  spawn player aerial unit");
            }
        }



    }

}
