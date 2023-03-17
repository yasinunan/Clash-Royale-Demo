using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UOP1.Helper
{
    public class SpellAttackPositioner : MonoBehaviour
    {
        Transform target;

        [SerializeField] private float speed = 5f;
        [SerializeField] private bool canAttack = false;

        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnLongRangeAttack += OnLongRangeAttack;

        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnLongRangeAttack -= OnLongRangeAttack;

        }

        //===============================================================================================

        void Update()
        {
            if (canAttack)
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);


                if (Vector3.Distance(transform.position, target.position) < 0.001f)
                {
                    canAttack = false;
                    PoolingManager.Instance.ReleasePooledObject(this.gameObject);
                }
            }

        }

        //===============================================================================================

        private void OnLongRangeAttack(GameObject gameObjectToFire, Transform _target)
        {
            if (ReferenceEquals(gameObjectToFire, this.gameObject))
            {
                /*transform.DOMove(target.transform.position, duration).OnComplete(() =>
                {
                    PoolingManager.Instance.ReleasePooledObject(this.gameObject);
                });*/

                target = _target;
                canAttack = true;

            }
        }
    }
}
