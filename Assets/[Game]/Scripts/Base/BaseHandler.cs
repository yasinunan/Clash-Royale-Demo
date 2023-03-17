using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UOP1.Helper
{
    public class BaseHandler : MonoBehaviour
    {

        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;
        [SerializeField] private bool isThisMainBase = false;
        [SerializeField] HealthBarHandler healthBarHandler;


        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnAttackSingleUnit += OnAttackSingleUnit;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnAttackSingleUnit -= OnAttackSingleUnit;

        }

        //===============================================================================================

        void Awake()
        {
            healthBarHandler = GetComponent<HealthBarHandler>();
        }

        //===============================================================================================

        void Start()
        {

        }

        //===============================================================================================


        void Update()
        {

        }

        private void OnAttackSingleUnit(GameObject attackTarget, float damageAmount)
        {
            if (ReferenceEquals(this.gameObject, attackTarget))
            {
                if (currentHealth > damageAmount)
                {
                    currentHealth -= damageAmount;
                    healthBarHandler.HealthChanged(currentHealth, maxHealth);
                }
                else
                {
                    currentHealth = 0f;
                    healthBarHandler.HealthChanged(currentHealth, maxHealth);

                    if (isThisMainBase)
                    {
                        LevelManager.Instance.MainBaseDestroyed();
                        this.gameObject.SetActive(false);
                    }
                    else
                    {
                        LevelManager.Instance.SideBaseDestroyed(this.gameObject);
                        this.gameObject.SetActive(false);
                    }
                }

            }
        }
    }

}
