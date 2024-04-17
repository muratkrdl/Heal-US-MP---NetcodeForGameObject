using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageMultiplier : MonoBehaviour
{
    public static DamageMultiplier Instance { get; private set; }

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI liveText;
    [SerializeField] TextMeshProUGUI deadText;

    [SerializeField] Villager[] villagers;

    [SerializeField] float increaseAmountDamageMultiplierValue;
    float damageMultiplierValue;

    int liveCounter;

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

        countText.text = villagers.Length.ToString();
        UpdateText();
    }

    public float GetDamageMultiplierValue
    {
        get
        {
            return damageMultiplierValue;
        }
    }

    public void UpdateText()
    {
        liveCounter = 0;
        foreach (Villager villager in villagers)
        {
            //if(villager.GetIsDead) { continue; }
            liveCounter++;
        }
        damageMultiplierValue = liveCounter * increaseAmountDamageMultiplierValue;
        liveText.text = liveCounter.ToString();
        deadText.text = (villagers.Length - liveCounter).ToString();
        if(liveCounter < Mathf.RoundToInt(villagers.Length / 2 - 1))
        {
            GameManager.Instance.LoseGame();
        }
    }

}
