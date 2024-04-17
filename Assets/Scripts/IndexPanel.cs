using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndexPanel : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI hpIndexText;

    [Header("Mana")]
    [SerializeField] Slider manaSlider;
    [SerializeField] TextMeshProUGUI manaIndexText;

    [Header("Stamina")]
    [SerializeField] Slider staminaSlider;
    [SerializeField] TextMeshProUGUI staminaIndexText;

    void Start() 
    {
        StartCoroutine(UpdateCurrentValues());
    }

    IEnumerator UpdateCurrentValues()
    {
        while(true)
        {
            yield return new WaitForSeconds(.1f);
            hpIndexText.text = ((int)hpSlider.value).ToString() + " / " + hpSlider.maxValue.ToString();

            manaIndexText.text = Mathf.RoundToInt(manaSlider.value).ToString() + " / " + manaSlider.maxValue.ToString();

            staminaIndexText.text = ((int)staminaSlider.value).ToString() + " / " + staminaSlider.maxValue.ToString();
        }
    }

}
