using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUIController : MonoBehaviour
{
    public GameObject UI;
    public Image healthBarFill;

    public float timeToFillFullBar = 2f;
    private float timeToFillPartBar;

    private float totalHealth = 100f;

    private float finalValue;
    private Coroutine changeHealthBarValueCoroutine;

    public void SetTotalHealth(float value)
    {
        totalHealth = value;
    }

    //Se la pasa una cantidad positiva para aumentar su valor y una cantidad negativa para disminuirlo
    public void ChangeHealthBarValue(float amount)
    {
        if(changeHealthBarValueCoroutine != null)
        {
            StopCoroutine(changeHealthBarValueCoroutine);
            float remainingAmount = finalValue - healthBarFill.fillAmount;
            float realAmount = amount/totalHealth;
            float newAmount = remainingAmount + realAmount;

            if(healthBarFill.fillAmount + newAmount > 1f) newAmount = 1 - healthBarFill.fillAmount;
            else if(healthBarFill.fillAmount + newAmount < 0f) newAmount = -healthBarFill.fillAmount;

            timeToFillPartBar = Mathf.Abs(newAmount) * timeToFillFullBar;
            finalValue = healthBarFill.fillAmount + newAmount;

            changeHealthBarValueCoroutine = StartCoroutine(ChangeHealthBarValueCoroutine());
        }
        else
        {
            float realAmount = amount/totalHealth; //Regla de 3 para que la cantidad a aumentar o disminuir sea sobre 1
            
            if(healthBarFill.fillAmount + realAmount > 1) realAmount = 1 - healthBarFill.fillAmount;
            else if(healthBarFill.fillAmount + realAmount < 0) realAmount = -healthBarFill.fillAmount;

            //Regla de 3 para calcular cuánto tiempo requiere llenar o vaciar esa cantidad en función del tiempo que requeriría llenar la barra completa
            timeToFillPartBar = Mathf.Abs(realAmount) * timeToFillFullBar;

            finalValue = healthBarFill.fillAmount + realAmount;

            changeHealthBarValueCoroutine = StartCoroutine(ChangeHealthBarValueCoroutine());
        }
    }

    IEnumerator ChangeHealthBarValueCoroutine()
    {
        float elapsedTime = 0.0f;

        float initialValue = healthBarFill.fillAmount;

        while(elapsedTime < timeToFillPartBar)
        {
            elapsedTime += Time.deltaTime;
            healthBarFill.fillAmount = Mathf.Lerp(initialValue, finalValue, elapsedTime / timeToFillPartBar);
            yield return null;
        }
        healthBarFill.fillAmount = finalValue;

        changeHealthBarValueCoroutine = null;
    }

    public void EnableUI(bool value)
    {
        UI.SetActive(value);
    }
}
