using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


namespace UOP1.Helper
{
    public class PlayerAerialUnit : MonoBehaviour
    {
        [SerializeField] UnitData data;

        private const string TRIGGER_ANIM_RUN = "triggerAnimRun";
        private const string TRIGGER_ANIM_ATTACK = "triggerAnimAttack";

        private Coroutine attackCoroutine;
        [SerializeField] List<GameObject> sideBases;
        [SerializeField] GameObject mainBase;

        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private GameObject target, targetGroundUnit, targetAerialUnit, targetBase;

        [SerializeField] private List<GameObject> closestTargetsList;
        [SerializeField] private HealthBarHandler healthBarHandler;
        [SerializeField] private Animator animator;

        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private float damageAmount = 1f;
        [SerializeField] private float attackTime = 1f;
        [SerializeField] private float minAttackRange = 2f;

        [SerializeField] private bool isAttackAnimPlaying = false;
        [SerializeField] private bool isRunAnimPlaying = true;
        [SerializeField] private bool canAttack;
        [SerializeField] private bool canAttackAerialUnits;



        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnEnemySideBaseDestroyed += OnEnemySideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit += OnAttackSingleUnit;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnEnemySideBaseDestroyed -= OnEnemySideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit -= OnAttackSingleUnit;

            healthBarHandler.ResetProgressBar();
            currentHealth = maxHealth;
        }

        //===============================================================================================

        void Awake()
        {
            healthBarHandler = GetComponent<HealthBarHandler>();
            currentHealth = maxHealth;
            agent = GetComponent<NavMeshAgent>();
        }

        //===============================================================================================

        private void Start()
        {
            //sideBases.Add(GameObject.FindGameObjectsWithTag("Base").ToList());
            sideBases = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyBase"));
            mainBase = GameObject.FindGameObjectWithTag("EnemyMainBase");
        }

        //===============================================================================================

        private void FixedUpdate()
        {
            /* string[] collidableLayers = { strLayerGroundUnit, strLayerLongRangeUnit, strLayerBase, strLayerMainBase };
             int intLayersToCheck = LayerMask.GetMask(collidableLayers);


             if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, raycastMaxDistance, intLayersToCheck))
             {
                 isEnemyInAttackRange = true;
                 Debug.Log("Target Detected!");

             }
             else
             {
                 isEnemyInAttackRange = false;
                 Debug.Log("Target Not Detected!");
             }*/

        }

        //===============================================================================================


        void Update()
        {
            if (!LevelManager.Instance.isGameStarted)
            {
                return;
            }


            FindTarget();
            agent.SetDestination(target.transform.position);


            if (Vector3.Distance(transform.position, target.transform.position) <= minAttackRange)
            {
                if (!isAttackAnimPlaying)
                {
                    isRunAnimPlaying = false;
                    isAttackAnimPlaying = true;
                    animator.SetTrigger(TRIGGER_ANIM_ATTACK);
                    if (attackCoroutine != null)
                    {
                        StopCoroutine(attackCoroutine);
                    }
                    attackCoroutine = StartCoroutine(Attack());
                }
            }
            else
            {
                if (!isRunAnimPlaying)
                {
                    isRunAnimPlaying = true;
                    isAttackAnimPlaying = false;
                    animator.SetTrigger(TRIGGER_ANIM_RUN);

                    StopCoroutine(attackCoroutine);
                }
            }

        }

        //===============================================================================================

        private IEnumerator Attack()
        {
            while (true)
            {
                yield return new WaitForSeconds(attackTime);
                LevelManager.Instance.AttackSingleUnit(target, damageAmount);
            }
        }

        //===============================================================================================

        private void FindTarget()
        {
            closestTargetsList.Clear();

            if (EnemySpawner.Instance.groundedEnemyList.Count > 0)
            {
                targetGroundUnit = SortClosestObjectInList(EnemySpawner.Instance.groundedEnemyList);
                closestTargetsList.Add(targetGroundUnit);
            }
            if (canAttackAerialUnits && EnemySpawner.Instance.aerialEnemyList.Count > 0)
            {
                targetAerialUnit = SortClosestObjectInList(EnemySpawner.Instance.aerialEnemyList);
                closestTargetsList.Add(targetAerialUnit);
            }
            if (sideBases.Count > 0)
            {
                targetBase = SortClosestObjectInList(sideBases);
                closestTargetsList.Add(targetBase);
            }
            if (closestTargetsList.Count > 0)
            {
                target = SortClosestObjectInList(closestTargetsList);
            }
            else
            { target = mainBase; }
        }

        //===============================================================================================

        private GameObject SortClosestObjectInList(List<GameObject> objectList)
        {
            GameObject gameObject = objectList.OrderBy(go => (transform.position - go.transform.position).sqrMagnitude).First().transform.gameObject;
            return gameObject;
        }

        //===============================================================================================

        private void OnEnemySideBaseDestroyed(GameObject sideBase)
        {
            sideBases.Remove(sideBase);
        }

        //===============================================================================================

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

                    PlayerUnitSpawner.Instance.aerialPlayerList.Remove(this.gameObject);
                    PoolingManager.Instance.ReleasePooledObject(this.gameObject);
                }

            }
        }
    }
}

