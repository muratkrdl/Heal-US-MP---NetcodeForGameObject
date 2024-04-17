using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class XPPanel : MonoBehaviour
{
    public static XPPanel Instance { get; private set; }

    [SerializeField] GameObject levelUpPanel;

    [SerializeField] Slider xpSlider;

    [SerializeField] float maxXP;
    [SerializeField] float lerpTime;

    [SerializeField] float increaseAmountMaxXP;

    [SerializeField] StarterAssetsInputs starterAssetsInputs;

    float currentXP;

    float targetXP;

    public float MaxXP
    {
        get
        {
            return maxXP;
        }
        set
        {
            maxXP = value;
        }
    }

    public float GetCurrentHP
    {
        get
        {
            return currentXP;
        }
    }

    void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        xpSlider.maxValue = maxXP;
        currentXP = 0;
        targetXP = currentXP;
        xpSlider.value = currentXP;
    }

/*
    void Update() 
    {
        if(GameManager.Instance.GetGoingHome) { return; }
        if(levelUpPanel.GetComponent<LevelUpPanel>().IsThinking)
        {
            if(Time.timeScale != 0)
                Time.timeScale = 0;
            if(Cursor.lockState == CursorLockMode.Locked)
                starterAssetsInputs.SetCursorState(false);
        }
        else
        {
            if(Time.timeScale != 1)
                Time.timeScale = 1;
            if(Cursor.lockState == CursorLockMode.None)
                starterAssetsInputs.SetCursorState(true);
        }
    }
*/
    public void IncreaseXP(float amount)
    {
        StartCoroutine(SmoothHPVisual(amount));
    }

    IEnumerator SmoothHPVisual(float amount)
    {
        targetXP += amount;
        while(true)
        {
            yield return null;
            if(amount > 0 && currentXP >= maxXP) { currentXP = maxXP; targetXP = currentXP; } 
            currentXP = Mathf.Lerp(currentXP, targetXP, Time.deltaTime * lerpTime);
            xpSlider.value = currentXP;

            if(Mathf.Abs(currentXP - targetXP) <= .01f)
            {
                xpSlider.value = targetXP;
                break;
            }
        }
        if(Mathf.Abs(currentXP - maxXP) <= .2f)
        {
            currentXP = 0;
            targetXP = 0;
            maxXP += increaseAmountMaxXP;
            SoundManager.Instance.PlaySound3D("Level Up",GameManager.Instance.GetPlayer.position);
            UpdateVisual();
            levelUpPanel.SetActive(true);
        }
        StopAllCoroutines();
    }

    public void UpdateVisual()
    {
        xpSlider.maxValue = maxXP;
        xpSlider.value = currentXP;
    }

}
