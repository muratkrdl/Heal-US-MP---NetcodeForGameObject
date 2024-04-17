using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PotionsAbstract : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    [SerializeField] Image BG;

    public int maxCount;
    public int currentCount;

    const int startCount = 1;

    public int GetMaxCount
    {
        get
        {
            return maxCount;
        }
    }

    public int GetCurrentCount
    {
        get
        {
            return currentCount;
        }
    }

    void Start() 
    {
        currentCount = startCount;
        SetPotionCount();
    }

    public void IncreasePotionCount()
    {
        currentCount++;
        SetPotionCount();
    }

    public void DecreasePotionCount()
    {
        currentCount--;
        SetPotionCount();
    }

    public void SetPotionCount()
    {
        potionCountText.text = currentCount.ToString();
        if(currentCount <= 0) 
            BG.enabled = true;
        else 
            BG.enabled = false;
    }

}
