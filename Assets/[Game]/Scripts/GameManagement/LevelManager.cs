using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.SceneManagement;


namespace UOP1.Helper
{
    [DefaultExecutionOrder(-1)]
    public class LevelManager : Singleton<LevelManager>
    {



        [HideInInspector] public bool isGameStarted;
        [HideInInspector] public bool isGameFinished;
        [HideInInspector] public bool isMainBaseDestroyed = false;
        [HideInInspector] public bool isEnemyMainBaseDestroyed = false;

        [SerializeField] private float restartTime = 0.5f;



        //===============================================================================================

        public delegate void OnGameStartedDelegate();
        public event OnGameStartedDelegate OnGameStarted;
        public delegate void OnGameFinishedDelegate();
        public event OnGameFinishedDelegate OnGameFinished;


        public delegate void OnUpdateCountDownDelegate(int time);
        public event OnUpdateCountDownDelegate OnUpdateCountDown;

        public delegate void OnStartCountDownDelegate(int time);
        public event OnStartCountDownDelegate OnStartCountDown;

        public delegate void OnTimeExpiredDelegate();
        public event OnTimeExpiredDelegate OnTimeExpired;



        public delegate void OnMainBaseDestroyedDelegate();
        public event OnMainBaseDestroyedDelegate OnMainBaseDestroyed;

        public delegate void OnSideBaseDestroyedDelegate(GameObject sideBase);
        public event OnSideBaseDestroyedDelegate OnSideBaseDestroyed;

        public delegate void OnEnemyMainBaseDestroyedDelegate();
        public event OnEnemyMainBaseDestroyedDelegate OnEnemyMainBaseDestroyed;

        public delegate void OnEnemySideBaseDestroyedDelegate(GameObject sideBase);
        public event OnEnemySideBaseDestroyedDelegate OnEnemySideBaseDestroyed;


        public delegate void OnAttackSingleUnitDelegate(GameObject attackTarget, float damageAmount);
        public event OnAttackSingleUnitDelegate OnAttackSingleUnit;
        public delegate void OnHealSingleUnitDelegate(GameObject healTarget, float healAmount);
        public event OnHealSingleUnitDelegate OnHealSingleUnit;


        public delegate void OnLongRangeAttackDelegate(GameObject arrow, Transform target);
        public event OnLongRangeAttackDelegate OnLongRangeAttack;

        public delegate void OnAnimationEventTriggeredDelegate(GameObject firingUnit);
        public event OnAnimationEventTriggeredDelegate OnAnimationEventTriggered;

        public delegate void OnLoadingFinishedDelegate();
        public event OnLoadingFinishedDelegate OnLoadingFinished;


        private void OnEnable()
        {

        }

        //===============================================================================================

        private void OnDisable()
        {

        }


        //===============================================================================================

        void Awake()
        {
            Application.targetFrameRate = 60;

            isGameStarted = false;
            isGameFinished = false;
        }

        //===============================================================================================

        private void Start()
        {
            //isGameStarted = true;
        }

        //===============================================================================================

        public void StartGame()
        {
            isGameStarted = true;
            OnLoadingFinished?.Invoke();
        }


        //===============================================================================================

        public void RestartGame()
        {
            StartCoroutine(Restarter());
        }
        //===============================================================================================


        public IEnumerator Restarter()
        {
            isGameFinished = true;

            yield return new WaitForSeconds(restartTime);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        //===============================================================================================




        //===============================================================================================

        ///
        /// Events
        ///

        public void GameStarted()
        {
            OnGameStarted?.Invoke();
        }

        //===============================================================================================

        public void GameFinished()
        {

            OnGameFinished?.Invoke();
        }

        //===============================================================================================

        public void UpdateCountDown(int time)
        {
            OnUpdateCountDown?.Invoke(time);
        }

        //===============================================================================================

        public void StartCountDown(int startTime)
        {
            OnStartCountDown?.Invoke(startTime);
        }

        //===============================================================================================

        public void TimeExpired()
        {
            OnTimeExpired?.Invoke();
        }

        //===============================================================================================

        public void MainBaseDestroyed()
        {
            PoolingManager.Instance.ReleaseAllPooledObjects();
            isGameStarted = false;
            isMainBaseDestroyed = true;
            OnMainBaseDestroyed?.Invoke();
        }

        //===============================================================================================

        public void SideBaseDestroyed(GameObject sideBase)
        {

            OnSideBaseDestroyed?.Invoke(sideBase);
        }

        //===============================================================================================

        public void EnemyMainBaseDestroyed()
        {
            isEnemyMainBaseDestroyed = true;
            OnEnemyMainBaseDestroyed?.Invoke();
        }

        //===============================================================================================

        public void EnemySideBaseDestroyed(GameObject sideBase)
        {

            OnEnemySideBaseDestroyed?.Invoke(sideBase);
        }

        //===============================================================================================

        public void AttackSingleUnit(GameObject attackTarget, float damageAmount)
        {
            OnAttackSingleUnit?.Invoke(attackTarget, damageAmount);
        }

        //===============================================================================================

        public void HealSingleUnit(GameObject _healTarget, float _healAmount)
        {
            OnHealSingleUnit?.Invoke(_healTarget, _healAmount);
        }

        //===============================================================================================

        public void LongRangeAttack(GameObject attackArrow, Transform target)
        {
            OnLongRangeAttack?.Invoke(attackArrow, target);
        }

        //===============================================================================================

        public void AnimationEventTriggered(GameObject firingUnit)
        {
            OnAnimationEventTriggered?.Invoke(firingUnit);
        }

    }

}
