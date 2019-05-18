using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public RectTransform healthBarFill;
    public RectTransform staminaBarFill;
    public RectTransform reloadGunBarFill;

    private float barMaxWidth;
    private float barHeight;

    private float newHealthBarWidth;
    private float newStaminaBarWidht;
    private float newReloadGunBarWidht;

    private Coroutine updateHealthBarCoroutine;
    private Coroutine updateStaminaBarCoroutine;
    private Coroutine updateReloadGunBarCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        barMaxWidth = healthBarFill.sizeDelta.x;
        barHeight = healthBarFill.sizeDelta.y;
    }

    public void UpdateHealthBar(float currentValue)
    {
        newHealthBarWidth = (currentValue * barMaxWidth) / 100;

        if(updateHealthBarCoroutine != null)
        {
            StopCoroutine(updateHealthBarCoroutine);
            updateHealthBarCoroutine = null;
            healthBarFill.sizeDelta = new Vector2(newHealthBarWidth, barHeight);
        }
        updateHealthBarCoroutine = StartCoroutine(UpdateHealthBarCoroutine(0.3f));
    }

    IEnumerator UpdateHealthBarCoroutine(float time)
    {
        float initialWidth = healthBarFill.sizeDelta.x;
        float auxWidth = 0.0f;
        
        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            auxWidth = Mathf.Lerp(initialWidth, newHealthBarWidth, elapsedTime / time);

            healthBarFill.sizeDelta = new Vector2(auxWidth, barHeight);

            yield return null;
        }
        healthBarFill.sizeDelta = new Vector2(newHealthBarWidth, barHeight);

        updateHealthBarCoroutine = null;
    }

    public void UpdateStaminaBar(float currentValue)
    {
        newStaminaBarWidht = (currentValue * barMaxWidth) / 100;

        if(updateStaminaBarCoroutine != null)
        {
            StopCoroutine(updateStaminaBarCoroutine);
            updateStaminaBarCoroutine = null;
            staminaBarFill.sizeDelta = new Vector2(newStaminaBarWidht, barHeight);
        }
        updateStaminaBarCoroutine = StartCoroutine(UpdateStaminaBarCoroutine(0.3f));
    }

    IEnumerator UpdateStaminaBarCoroutine(float time)
    {
        float initialWidth = staminaBarFill.sizeDelta.x;
        float auxWidth = 0.0f;
        
        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            auxWidth = Mathf.Lerp(initialWidth, newStaminaBarWidht, elapsedTime / time);

            staminaBarFill.sizeDelta = new Vector2(auxWidth, barHeight);

            yield return null;
        }
        staminaBarFill.sizeDelta = new Vector2(newStaminaBarWidht, barHeight);

        updateStaminaBarCoroutine = null;
    }

    public void UpdateReloadGunBar(float currentValue)
    {
        newReloadGunBarWidht = (currentValue * barMaxWidth) / 100;

        if(updateReloadGunBarCoroutine != null)
        {
            StopCoroutine(updateReloadGunBarCoroutine);
            updateReloadGunBarCoroutine = null;
            reloadGunBarFill.sizeDelta = new Vector2(newReloadGunBarWidht, barHeight);
        }
        updateReloadGunBarCoroutine = StartCoroutine(UpdateReloadGunBarCoroutine(0.3f));
    }

    IEnumerator UpdateReloadGunBarCoroutine(float time)
    {
        float initialWidth = reloadGunBarFill.sizeDelta.x;
        float auxWidth = 0.0f;
        
        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            auxWidth = Mathf.Lerp(initialWidth, newReloadGunBarWidht, elapsedTime / time);

            reloadGunBarFill.sizeDelta = new Vector2(auxWidth, barHeight);

            yield return null;
        }
        reloadGunBarFill.sizeDelta = new Vector2(newReloadGunBarWidht, barHeight);

        updateReloadGunBarCoroutine = null;
    }
}
