using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

namespace UOP1.Helper
{
    public class EnemySpawner : Singleton<EnemySpawner>
    {
        private const string ENEMY_GROUND_UNIT = "EnemyGroundUnit";
        private const string ENEMY_AERIAL_UNIT = "EnemyAerialUnit";
        private const string ENEMY_LONGRANGE_UNIT = "EnemyLongRangeUnit";

        [SerializeField] Transform unitHolderTransform;

        public List<GameObject> groundedEnemyList;
        public List<GameObject> aerialEnemyList;

        private int randomEnemyUnit;
        [SerializeField] private int minRandomRange = 0;
        [SerializeField] private int maxRandomRange = 3;
        [SerializeField] private float horizontalSpawnRange = 8f;
        [SerializeField] private float spawnZDistance = 42f;





        [SerializeField] private float enemySpawnRate = 5f;
        [SerializeField] private float enemySpawnRateUpdateTime = 5f;



        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        private void Start()
        {
            StartCoroutine(SpawnEnemy());
            StartCoroutine(UpdateSpawnRate());
        }

        /// <summary>
        /// this function makes enemies spawn more frequently as the time goes by;
        /// 
        /// </summary>
        private IEnumerator UpdateSpawnRate()
        {
            while (true)
            {


                yield return new WaitForSeconds(enemySpawnRateUpdateTime);
                if (LevelManager.Instance.isGameStarted)
                {
                    if (enemySpawnRate > 1f)
                    {
                        enemySpawnRate--;
                    }
                }
            }

        }

        /// <summary>
        /// this function makes enemies spawn more frequently as the time goes by;
        /// 
        /// </summary>
        private IEnumerator SpawnEnemy()
        {
            while (true)
            {

                int randomUnit = Random.Range(minRandomRange, maxRandomRange);

                yield return new WaitForSeconds(enemySpawnRate);

                if (LevelManager.Instance.isGameStarted)
                {
                    if (randomUnit == 0)
                    {
                        GetPooledObject(ENEMY_GROUND_UNIT, false);

                    }
                    else if (randomUnit == 1)
                    {
                        GetPooledObject(ENEMY_AERIAL_UNIT, true);
                    }
                    else if (randomUnit == 2)
                    {
                        GetPooledObject(ENEMY_LONGRANGE_UNIT, false);
                    }
                }

            }
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        private void GetPooledObject(string unit, bool isEnemyAerial)
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(unit);
            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = new Vector3(Random.Range(-horizontalSpawnRange, horizontalSpawnRange), 0f, spawnZDistance);

                if (isEnemyAerial)
                {
                    aerialEnemyList.Add(go);
                }
                else
                {
                    groundedEnemyList.Add(go);
                }
            }
            else
            {
                Debug.Log("Could not found enemy unit");
            }
        }


    }

}
