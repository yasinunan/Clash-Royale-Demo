using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


namespace UOP1.Helper
{
    public class PlayerUnit : MonoBehaviour
    {
        [SerializeField] UnitData data;
        private const string TRIGGER_ANIM_RUN = "triggerAnimRun";
        private const string SPELL_ATTACK = "SpellAttack";
        private const string TRIGGER_ANIM_ATTACK = "triggerAnimAttack";
        private const string TRIGGER_ANIM_HEAL = "triggerAnimHeal";

        private HealthBarHandler healthBarHandler;
        private Animator animator;
        private Coroutine coroutine;
        private NavMeshAgent agent;


        [SerializeField] private GameObject target, targetGroundUnit, targetAerialUnit, targetBase, mainBase;
        [SerializeField] private Transform unitHolderTransform;
        [SerializeField] private List<GameObject> sideBases;
        [SerializeField] private List<GameObject> closestTargetsList;


        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;
        [SerializeField] private float damageAmount = 1f;
        [SerializeField] private float healAmount;
        [SerializeField] private float attackTime = 1f;
        [SerializeField] private float speed;
        [SerializeField] private float stoppingDistance;
        [SerializeField] private float minAttackRange = 2f;
        [SerializeField] private float buffAmount;
        [SerializeField] private float buffTime;
        [SerializeField] private float manaCost;
        [SerializeField] private float baseOffset;
       

        [SerializeField] private bool isAttackAnimPlaying = false;
        [SerializeField] private bool isHealAnimPlaying = false;
        [SerializeField] private bool isRunAnimPlaying = true;
        [SerializeField] private bool canAttack;
        [SerializeField] private bool canSupport;
        [SerializeField] private bool canAttackAerialUnits;


        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnSideBaseDestroyed += OnSideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit += OnAttackSingleUnit;
            LevelManager.Instance.OnHealSingleUnit += OnHealSingleUnit;
            LevelManager.Instance.OnAnimationEventTriggered += OnAnimationEventTriggered;
        }

        //===============================================================================================

        void OnDisable()
        {
            LevelManager.Instance.OnSideBaseDestroyed -= OnSideBaseDestroyed;
            LevelManager.Instance.OnAttackSingleUnit -= OnAttackSingleUnit;
            LevelManager.Instance.OnHealSingleUnit -= OnHealSingleUnit;
            LevelManager.Instance.OnAnimationEventTriggered -= OnAnimationEventTriggered;

            healthBarHandler.ResetProgressBar();
            currentHealth = maxHealth;
        }

        //===============================================================================================

        void Awake()
        {
            healthBarHandler = GetComponent<HealthBarHandler>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();

            sideBases = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyBase"));
            mainBase = GameObject.FindGameObjectWithTag("EnemyMainBase");

        }

        //===============================================================================================

        private void Start()
        {
            baseOffset = data.baseOffset;
            agent.baseOffset = baseOffset;
            speed = data.speed;
            agent.speed = speed;
            stoppingDistance = data.stoppingDistance;
            agent.stoppingDistance = stoppingDistance;

            maxHealth = data.maxHealth;
            currentHealth = maxHealth;
            canAttack = data.canAttack;
            canAttackAerialUnits = data.canAttackAerialUnits;
            canSupport = data.canSupport;
            damageAmount = data.damageAmount;
            healAmount = data.healAmount;
            attackTime = data.attackTime;
            minAttackRange = data.minAttackRange;
            buffAmount = data.buffAmount;
            buffTime = data.buffTime;
            manaCost = data.manaCost;

        }

        //===============================================================================================


        void Update()
        {
            if (!LevelManager.Instance.isGameStarted)
            {
                return;
            }

            switch (data.characterType)
            {
                case UnitData.CharacterType.MELEE:

                    FindTarget(canAttackAerialUnits);
                    agent.SetDestination(target.transform.position);
                    AttackIfPossible();
                    break;

                case UnitData.CharacterType.RANGE:
                    FindTarget(canAttackAerialUnits);
                    agent.SetDestination(target.transform.position);
                    AttackIfPossible();
                    break;

                case UnitData.CharacterType.AERIAL:
                    FindTarget(canAttackAerialUnits);
                    agent.SetDestination(target.transform.position);
                    AttackIfPossible();
                    break;

                case UnitData.CharacterType.SUPPORT:
                    FindTargetToHeal();
                    agent.SetDestination(target.transform.position);

                    break;

            }


            //float step = rotateSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, target.transform.rotation, step);

        }

        //===============================================================================================

        private void AttackIfPossible()
        {

            if (target.activeSelf && Vector3.Distance(transform.position, target.transform.position) <= minAttackRange)
            {
                if (!isAttackAnimPlaying)
                {
                    isRunAnimPlaying = false;
                    isAttackAnimPlaying = true;
                    animator.SetTrigger(TRIGGER_ANIM_ATTACK);
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(Attack());
                }
            }
            else
            {
                if (!isRunAnimPlaying)
                {
                    isRunAnimPlaying = true;
                    isAttackAnimPlaying = false;
                    animator.SetTrigger(TRIGGER_ANIM_RUN);

                    StopCoroutine(coroutine);
                }
            }
        }

        //===============================================================================================

        private void HealIfPossible()
        {

            if (Vector3.Distance(transform.position, target.transform.position) <= minAttackRange)
            {
                if (!isHealAnimPlaying)
                {
                    isRunAnimPlaying = false;
                    isHealAnimPlaying = true;
                    animator.SetTrigger(TRIGGER_ANIM_HEAL);
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }
                    coroutine = StartCoroutine(Attack());
                }
            }
            else
            {
                if (!isRunAnimPlaying)
                {
                    isRunAnimPlaying = true;
                    isHealAnimPlaying = false;
                    animator.SetTrigger(TRIGGER_ANIM_RUN);

                    StopCoroutine(coroutine);
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

        private IEnumerator Heal()
        {
            while (true)
            {
                yield return new WaitForSeconds(attackTime);

                LevelManager.Instance.HealSingleUnit(target, healAmount);
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

        private void FindTarget(bool _canAttackAerialUnits)
        {
            closestTargetsList.Clear();
            if (_canAttackAerialUnits)
            {
                if (EnemySpawner.Instance.aerialEnemyList.Count > 0)
                {
                    targetAerialUnit = SortClosestObjectInList(EnemySpawner.Instance.aerialEnemyList);
                    closestTargetsList.Add(targetAerialUnit);
                }
            }
            if (EnemySpawner.Instance.groundedEnemyList.Count > 0)
            {
                targetGroundUnit = SortClosestObjectInList(EnemySpawner.Instance.groundedEnemyList);
                closestTargetsList.Add(targetGroundUnit);
            }
            if (sideBases.Count > 0)
            {
                targetBase = SortClosestObjectInList(sideBases);
                if (targetBase.activeSelf)
                {
                    closestTargetsList.Add(targetBase);
                }
                else
                {
                    sideBases.Remove(targetBase);

                }

            }
            if (closestTargetsList.Count > 0)
            {
                target = SortClosestObjectInList(closestTargetsList);
            }
            else
            { target = mainBase; }
        }

        //===============================================================================================

        private void FindTargetToHeal()
        {
            closestTargetsList.Clear();

            if (PlayerUnitSpawner.Instance.aerialPlayerList.Count > 0)
            {
                targetAerialUnit = SortClosestObjectInList(PlayerUnitSpawner.Instance.aerialPlayerList);
                closestTargetsList.Add(targetAerialUnit);
            }

            if (PlayerUnitSpawner.Instance.groundedPlayerList.Count > 0)
            {
                targetGroundUnit = SortClosestObjectInList(PlayerUnitSpawner.Instance.groundedPlayerList);
                closestTargetsList.Add(targetGroundUnit);
            }

            if (closestTargetsList.Count > 0)
            {
                target = SortClosestObjectInList(closestTargetsList);
            }
        }

        //===============================================================================================

        private GameObject SortClosestObjectInList(List<GameObject> objectList)
        {
            GameObject gameObject = objectList.OrderBy(go => (transform.position - go.transform.position).sqrMagnitude).First().transform.gameObject;
            return gameObject;
        }

        //===============================================================================================

        private void OnSideBaseDestroyed(GameObject _sideBase)
        {
            for (int i = 0; i < sideBases.Count; i++)
            {
                if (ReferenceEquals(sideBases[i].gameObject, _sideBase))
                {
                    sideBases.Remove(sideBases[i]);

                }
            }


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

                    switch (data.characterType)
                    {
                        case UnitData.CharacterType.MELEE:
                            PlayerUnitSpawner.Instance.groundedPlayerList.Remove(this.gameObject);
                            PoolingManager.Instance.ReleasePooledObject(this.gameObject);

                            break;

                        case UnitData.CharacterType.RANGE:
                            PlayerUnitSpawner.Instance.groundedPlayerList.Remove(this.gameObject);
                            PoolingManager.Instance.ReleasePooledObject(this.gameObject);

                            break;

                        case UnitData.CharacterType.AERIAL:
                            PlayerUnitSpawner.Instance.aerialPlayerList.Remove(this.gameObject);
                            PoolingManager.Instance.ReleasePooledObject(this.gameObject);

                            break;

                        case UnitData.CharacterType.SUPPORT:
                            PlayerUnitSpawner.Instance.groundedPlayerList.Remove(this.gameObject);
                            PoolingManager.Instance.ReleasePooledObject(this.gameObject);

                            break;

                    }

                }

            }
        }

        //===============================================================================================

        private void OnHealSingleUnit(GameObject healTarget, float healAmount)
        {

            if (ReferenceEquals(this.gameObject, healTarget))
            {
                currentHealth += healAmount;
                healthBarHandler.HealthChanged(currentHealth, maxHealth);
            }
        }

        //===============================================================================================

        private void OnAnimationEventTriggered(GameObject animEventTriggerParent)
        {
            if (ReferenceEquals(animEventTriggerParent, this.gameObject))
            {
                SpawnLongRangeAttack();
            }
        }
    }
}

