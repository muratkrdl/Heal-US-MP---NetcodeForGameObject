using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseStaminaPotion : MonoBehaviour
{
    [SerializeField] float staminaAmount;
    [SerializeField] StaminaPotions staminaPotions;

    PlayerManager playerManager;

    void Start() 
    {
        playerManager = GetComponentInParent<FirstPersonController>().GetPlayerManager;     
    }

    public void UseStaminaPotionFunc()
    {
        if(Stamina.Instance.GetCurrentStamina >= Stamina.Instance.MaxStamina -1 || staminaPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        Stamina.Instance.IncreaseStaminaWithPotion(staminaAmount);
        staminaPotions.DecreasePotionCount();
    }

}
