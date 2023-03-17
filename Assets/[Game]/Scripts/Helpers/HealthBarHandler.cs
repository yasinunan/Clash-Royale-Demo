using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UOP1.Helper
{
    public class HealthBarHandler : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        Coroutine healthBarCoroutine;
    
        void Start()
        {

        }


        void Update()
        {

        }
        private void SetProgressBar(float currentHealth, float maxHealth)
        {

            //healthBar.fillAmount = currentHealth / maxHealth;
            if (healthBarCoroutine != null)
            {
                StopCoroutine(healthBarCoroutine);
            }
            healthBarCoroutine = StartCoroutine(ChangeImageFil(healthBar, currentHealth / maxHealth));

        }

        public  void ResetProgressBar()
        {

            healthBar.fillAmount = 1f;
        }

        public void HealthChanged(float currentHealth, float maxHealth)
        {
            //Debug.Log("HEALTH CHANGED");
            SetProgressBar(currentHealth, maxHealth);
        }

       
        IEnumerator ChangeImageFil(Image image, float value)
        {
            float elapsedTime = 0f;
            float waitTime = 0.6f;
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
