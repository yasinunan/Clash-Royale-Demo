using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


namespace UOP1.Helper
{
    public class EnemyLRangeUnit : MonoBehaviour
    {
        [SerializeField] UnitData data;
        private const string TRIGGER_ANIM_RUN = "triggerAnimRun";
        private const string SPELL_ATTACK = "SpellAttack";
        private const string TRIGGER_ANIM_ATTACK = "triggerAnimAttack";


        private Coroutine attackCoroutine;
        [SerializeField] List<GameObject> sideBases;
        [SerializeField] GameObject mainBase;
        [SerializeField] private Transform unitHolderTransform;
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
            LevelManager.Instance.OnSideBaseDestroyed += OnSideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit += OnAttackSingleUnit;
            LevelManager.Instance.OnAnimationEventTriggered += OnAnimationEventTriggered;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnSideBaseDestroyed -= OnSideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit -= OnAttackSingleUnit;
            LevelManager.Instance.OnAnimationEventTriggered -= OnAnimationEventTriggered;


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
            sideBases = new List<GameObject>(GameObject.FindGameObjectsWithTag("Base"));
            mainBase = GameObject.FindGameObjectWithTag("MainBase");
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

        private void SpawnLongRangeAttack()
        {
            GameObject go = PoolingManager.Instance.GetPooledObject(SPELL_ATTACK);
            if (go != null)
            {
                go.SetActive(true);
                go.transform.parent = unitHolderTransform;
                go.transform.position = this.transform.position + new Vector3(0, 1f, 0);
                LevelManager.Instance.LongRangeAttack(go, target.transform);

            }
            else
            {
                Debug.Log("Could not spawn arrow");
            }
        }


        //===============================================================================================

        private void FindTarget()
        {
            closestTargetsList.Clear();

            if (PlayerUnitSpawner.Instance.groundedPlayerList.Count > 0)
            {
                targetGroundUnit = SortClosestObjectInList(PlayerUnitSpawner.Instance.groundedPlayerList);
                closestTargetsList.Add(targetGroundUnit);
            }
            if (canAttackAerialUnits && PlayerUnitSpawner.Instance.aerialPlayerList.Count > 0)
            {
                targetAerialUnit = SortClosestObjectInList(PlayerUnitSpawner.Instance.aerialPlayerList);
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

        private void OnSideBaseDestroyed(GameObject sideBase)
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

                    EnemySpawner.Instance.groundedEnemyList.Remove(this.gameObject);
                    PoolingManager.Instance.ReleasePooledObject(this.gameObject);
                }

            }
        }

        private void OnAnimationEventTriggered(GameObject animEventTriggerParent)
        {
            if (ReferenceEquals(animEventTriggerParent, this.gameObject))
            {
                SpawnLongRangeAttack();
            }
        }
    }
}

