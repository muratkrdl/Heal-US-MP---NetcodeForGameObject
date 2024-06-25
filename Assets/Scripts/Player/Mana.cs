using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mana : NetworkBehaviour
{
    public static Mana Instance;

    [SerializeField] float maxMana;
    [SerializeField] float increaseSpeed;
    [SerializeField] float lerpTime;

    [SerializeField] Slider manaSlider;
    [SerializeField] TextMeshProUGUI valuesText;

    float currentMana;
    float targetMana;
    float previousMana;

    bool isChanging;

    public float MaxMana
    {
        get
        {
            return maxMana;
        }
        set
        {
            maxMana = value;
        }
    }

    public float GetCurrentMana
    {
        get
        {
            return currentMana;
        }
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            Instance = this;
        }
    }

    void Start() 
    {
        SetStartValues();    
    }

    void SetStartValues()
    {
        manaSlider.maxValue = maxMana;
        currentMana = maxMana;
        targetMana = currentMana;
        manaSlider.value = currentMana;
        UpdateCurrentValues();
    }

    void Update() 
    {
        if(currentMana >= maxMana || isChanging) { return; }
        if(currentMana < 0) 
        {
            currentMana = 0;
        }
        IncreaseManaWithTime();
    }

    public void IncreaseMana(float amount)
    {
        isChanging = true;
        StartCoroutine(SmoothManaVisual(amount));
    }

    public void DecreaseMana(float amount)
    {
        previousMana = currentMana - amount;
        if(previousMana < 0) { return; }
        isChanging = true;
        StopAllCoroutines();
        StartCoroutine(SmoothManaVisual(-amount));
    }

    void IncreaseManaWithTime()
    {
        if(currentMana >= maxMana || isChanging) { return; }
        currentMana += Time.deltaTime * increaseSpeed;
        Mathf.RoundToInt(currentMana);
        manaSlider.value = currentMana;
        if(currentMana >= maxMana - .1f)
        {
            currentMana = maxMana;
            manaSlider.value = maxMana;
        }
        UpdateCurrentValues();
    }

    IEnumerator SmoothManaVisual(float amount)
    {
        targetMana = currentMana;
        targetMana += amount;
        while(true)
        {
            yield return null;
            if(amount > 0 && currentMana >= maxMana) { currentMana = maxMana; targetMana = currentMana; } 
            currentMana = Mathf.Lerp(currentMana, targetMana, Time.deltaTime * lerpTime);
            manaSlider.value = currentMana;
            if(Mathf.Abs(currentMana - targetMana) <= .02f)
            {
                currentMana = targetMana;
                isChanging = false;
                UpdateCurrentValues();
                break;
            }
            UpdateCurrentValues();
        }
        StopAllCoroutines();
    }

    void UpdateCurrentValues()
    {
        valuesText.text = ((int)manaSlider.value).ToString() + " / " + manaSlider.maxValue.ToString();
    }

    public void UpdateValue()
    {
        manaSlider.maxValue = maxMana;
    }

}
