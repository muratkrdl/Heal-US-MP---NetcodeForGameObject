using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPoison : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI poisonCDText;
    [SerializeField] Image poisonBG;

    [SerializeField] Poison poisonPrefab;

    [SerializeField] FirstPersonController firstPersonController;

    [SerializeField] int abilityCD;
    [SerializeField] float manaCost;

    int currentAbilityCD;

    bool canUsePoison;


    public float GetManaCost
    {
        get
        {
            return manaCost;
        }
    }

    public bool GetCanUsePoison
    {
        get
        {
            return canUsePoison;
        }
    }

    void Awake() 
    {
        canUsePoison = true;
        currentAbilityCD = 0;
    }

    void Start() 
    {
        DisablePoisonCDCounter();
    }

    public void OpenPoison()
    {
        Mana.Instance.DecreaseMana(GetManaCost);

        OpenPoisonAbility();
    }

    void OpenPoisonAbility()
    {
        poisonPrefab.OpenPoison();

        StartCoroutine(PoisonAbilityCo()); // cd
    }

    public void ClosePoison()
    {
        poisonPrefab.ClosePoison();
    }

    IEnumerator PoisonAbilityCo()
    {
        canUsePoison = false;

        currentAbilityCD = abilityCD;
        EnablePoisonCDCounter();
        SetPoisonCDText();
      
        for (int i = 0; i < abilityCD; i++)
        {
            yield return new WaitForSeconds(1);
            currentAbilityCD--;
            SetPoisonCDText();
        }
       
        canUsePoison = true;
        StopCoroutine(PoisonAbilityCo());
    }

    void SetPoisonCDText()
    {
        poisonCDText.text = currentAbilityCD.ToString();
        if(currentAbilityCD == 0)
        {
            DisablePoisonCDCounter();
        }
    }

    void EnablePoisonCDCounter()
    {
        poisonCDText.enabled = true;
        poisonBG.enabled = true;
    }

    void DisablePoisonCDCounter()
    {
        poisonCDText.enabled = false;
        poisonBG.enabled = false;
    }

}
