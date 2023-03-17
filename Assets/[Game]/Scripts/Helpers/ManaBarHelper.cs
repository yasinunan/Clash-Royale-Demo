using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityEngine.UI;

namespace UOP1.Helper
{
    public class ManaBarHelper : Singleton<ManaBarHelper>
    {
        [SerializeField] private Image manaBar;
        [SerializeField] private float currentMana = 0f;
        [SerializeField] private float maxMana = 10f;
        Coroutine manaBarCoroutine;

        void Start()
        {
            ResetProgressBar();
        }


        void Update()
        {
            GainMana();
        }

        private void GainMana()
        {
            if (currentMana < maxMana)
            {
                currentMana += Time.deltaTime;
                //SetProgressBar(currentMana, maxMana);
                manaBar.fillAmount = currentMana / maxMana;
            }

        }

        public bool CanDragAndDrop(float mana)
        {
            if (mana <= currentMana)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void SetProgressBar(float _currentMana, float _maxMana)
        {

            //healthBar.fillAmount = currentHealth / maxHealth;
            if (manaBarCoroutine != null)
            {
                StopCoroutine(manaBarCoroutine);
            }
            manaBarCoroutine = StartCoroutine(ChangeImageFil(manaBar, _currentMana / _maxMana));

        }

        public void ResetProgressBar()
        {

            manaBar.fillAmount = 0f;
        }

        public void ManaChanged(float mana)
        {

            currentMana -= mana;
            SetProgressBar(currentMana, maxMana);

        }


        IEnumerator ChangeImageFil(Image image, float value)
        {
            float elapsedTime = 0f;
            float waitTime = 0.05f;
            while (elapsedTime < waitTime)
            {
                image.fillAmount = Mathf.Lerp(image.fillAmount, value, (elapsedTime / waitTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            image.fillAmount = value;
            yield return null;
        }
    }

}
