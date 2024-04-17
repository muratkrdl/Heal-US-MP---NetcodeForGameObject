using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPoison : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI poisonCDText;
    [SerializeField] Image poisonBG;

    [SerializeField] PhotonView PV;

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

        PV.RPC(nameof(OpenPoisonAbility),RpcTarget.All);
    }

    [PunRPC] void OpenPoisonAbility()
    {
        poisonPrefab.OpenPoison();

        if(PV.IsMine)
        {
            StartCoroutine(PoisonAbilityCo());
        }
    }

    public void ClosePoison()
    {
        PV.RPC(nameof(ClosePoisonAbility),RpcTarget.All);
    }

    [PunRPC] void ClosePoisonAbility()
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
