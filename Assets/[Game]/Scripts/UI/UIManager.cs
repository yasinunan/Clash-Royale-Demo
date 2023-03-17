using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace UOP1.Helper
{

    public class UIManager : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject gamePlayScreen;
        [SerializeField] private GameObject gameLoseScreen;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject gameWinScreen;



        //===============================================================================================

        void OnEnable()
        {
            LevelManager.Instance.OnUpdateCountDown += OnUpdateCountDown;
            LevelManager.Instance.OnTimeExpired += OnTimeExpired;
            LevelManager.Instance.OnMainBaseDestroyed += OnMainBaseDestroyed;
            LevelManager.Instance.OnLoadingFinished += OnLoadingFinished;
        }

        //===============================================================================================

        void OnDisable()
        {

            LevelManager.Instance.OnUpdateCountDown -= OnUpdateCountDown;
            LevelManager.Instance.OnTimeExpired -= OnTimeExpired;
            LevelManager.Instance.OnMainBaseDestroyed -= OnMainBaseDestroyed;
            LevelManager.Instance.OnLoadingFinished -= OnLoadingFinished;

        }

        //===============================================================================================

        void Start()
        {

        }

        //===============================================================================================
        private void OnUpdateCountDown(int time)
        {
            if (time >= 10)
            {
                timerText.text = "00:" + time.ToString();
            }
            else
            {
                timerText.text = "00:0" + time.ToString();
            }
        }

        //===============================================================================================

        private void OnLoadingFinished()
        {
            gamePlayScreen.SetActive(true);
            gameWinScreen.SetActive(false);
            gameLoseScreen.SetActive(false);
            loadingPanel.SetActive(false);
        }

        private void OnTimeExpired()
        {
            if (LevelManager.Instance.isMainBaseDestroyed)
            {
                gamePlayScreen.SetActive(false);
                gameWinScreen.SetActive(false);
                gameLoseScreen.SetActive(true);
            }
            else
            {
                gamePlayScreen.SetActive(false);
                gameWinScreen.SetActive(true);
                gameLoseScreen.SetActive(false);
            }
        }

        //===============================================================================================

        private void OnMainBaseDestroyed()
        {
            gamePlayScreen.SetActive(false);
            gameWinScreen.SetActive(false);
            gameLoseScreen.SetActive(true);
        }

    }
}
