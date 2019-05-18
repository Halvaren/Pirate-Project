using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public RectTransform healthBarFill;
    public RectTransform staminaBarFill;
    public RectTransform reloadGunBarFill;
    public Text moneyText;
    public Image keyImage;

    private float barMaxWidth;
    private float barHeight;

    private float newHealthBarWidth;
    private float newStaminaBarWidht;
    private float newReloadGunBarWidht;

    private Coroutine updateHealthBarCoroutine;
    private Coroutine updateStaminaBarCoroutine;
    private Coroutine updateReloadGunBarCoroutine;

    private float moneyTimer = 0.0f;
    private float timeBetweenMoneyUnitIncrease = 0.01f;
    private int currentMoney;
    private int newMoney;

    public Color keyEnabledColor;
    public Color keyDisabledColor;

    private bool aux = true;

    // Start is called before the first frame update
    void Start()
    {
        barMaxWidth = healthBarFill.sizeDelta.x;
        barHeight = healthBarFill.sizeDelta.y;

        currentMoney = 0;
        newMoney = currentMoney;
        moneyText.text = currentMoney.ToString();
    }

    void Update() 
    {
        if(currentMoney != newMoney)
        {
            moneyTimer += Time.deltaTime;
            if(moneyTimer >= timeBetweenMoneyUnitIncrease)
            {
                moneyTimer = 0f;
                currentMoney = currentMoney < newMoney ? currentMoney + 1 : currentMoney - 1;
                moneyText.text = currentMoney.ToString();
            }
        }
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

    public void IncreaseMoney(int amount)
    {
        newMoney += amount;
        if(newMoney < 0) newMoney = 0;
    }

    public void EnableKey(bool value)
    {
        if(value) keyImage.color = keyEnabledColor;
        else keyImage.color = keyDisabledColor;
    }
}
