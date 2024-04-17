using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public static Stamina Instance;

    [SerializeField] PhotonView PV;

    [SerializeField] float maxStamina;
    [SerializeField] float increaseSpeed;
    [SerializeField] float decreaseSpeed;

    [SerializeField] float waitForLoadTime;

    [SerializeField] KeyCode sprintKeyCode;

    [SerializeField] Slider staminaSlider;
    [SerializeField] TextMeshProUGUI valuesText;

    float currentStamina;

    bool waiting;
    bool canSprint;

    bool IsPressingLShift => Input.GetKey(sprintKeyCode);

    public Slider SetStaminaSlider
    {
        set
        {
            staminaSlider = value;
        }
    }

    public TextMeshProUGUI SetValuesText
    {
        set
        {
            valuesText = value;
        }
    }

    public float GetCurrentStamina
    {
        get
        {
            return currentStamina;
        }
    }

    public float MaxStamina
    {
        get
        {
            return maxStamina;
        }
        set
        {
            maxStamina = value;
        }
    }

    public bool GetCanSprint
    {
        get
        {
            return canSprint;
        }
    }

    void Awake() 
    {
        if(!PV.IsMine) { return; }
        Instance = this;
    }

    void Start() 
    {
        SetStartValues();  
    }

    void SetStartValues()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
        UpdateCurrentValues();
    }

    void Update() 
    {
        if(!PV.IsMine) { return; }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !waiting)
        {
            StopAllCoroutines();
            StartCoroutine(DecreaseStamina());
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartCoroutine(IncreaseStamina());
        }

        if(!IsPressingLShift) { canSprint = false; }
    }

    IEnumerator DecreaseStamina()
    {
        while(IsPressingLShift)
        {
            canSprint = true;
            yield return null;
            currentStamina -= Time.deltaTime * decreaseSpeed;
            staminaSlider.value = currentStamina;
            if(currentStamina <= staminaSlider.minValue + .2f)
            {
                StartCoroutine(WaitForLoad());
                StartCoroutine(IncreaseStamina());
                canSprint = false;
                break;
            }
            UpdateCurrentValues();
        }
        StopCoroutine(DecreaseStamina());
    }

    IEnumerator IncreaseStamina()
    {
        StopCoroutine(DecreaseStamina());
        while(true)
        {
            yield return null;
            currentStamina += Time.deltaTime * increaseSpeed;
            Mathf.RoundToInt(currentStamina);
            staminaSlider.value = currentStamina;
            if(currentStamina >= maxStamina - .1f)
            {
                currentStamina = maxStamina;
                staminaSlider.value = currentStamina;
                UpdateCurrentValues();
                break;
            }
            UpdateCurrentValues();
        }
        StopCoroutine(IncreaseStamina());
    }

    public void IncreaseStaminaWithPotion(float amount)
    {
        currentStamina += amount;
        staminaSlider.value = currentStamina;
        if(waiting) { waiting = false; StopCoroutine(WaitForLoad()); }
        if(currentStamina >= maxStamina) { currentStamina = maxStamina;}
    }

    IEnumerator WaitForLoad()
    {
        waiting = true;
        yield return new WaitForSeconds(waitForLoadTime);
        waiting = false;
        StopCoroutine(WaitForLoad());
    }

    void UpdateCurrentValues()
    {
        valuesText.text = ((int)staminaSlider.value).ToString() + " / " + staminaSlider.maxValue.ToString();
    }

    public void LevelUpMaxStamina()
    {
        if(currentStamina < maxStamina) { return; }
        StartCoroutine(IncreaseStamina());
    }

    public void UpdateMaxValue()
    {
        staminaSlider.maxValue = maxStamina;
    }

}
