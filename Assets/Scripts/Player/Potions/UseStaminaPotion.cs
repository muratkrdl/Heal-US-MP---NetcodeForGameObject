using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UseStaminaPotion : MonoBehaviour
{
    [SerializeField] float staminaAmount;
    [SerializeField] StaminaPotions staminaPotions;

    public void UseStaminaPotionFunc()
    {
        if(!GetComponentInParent<NetworkObject>().IsOwner) return;
        if(Stamina.Instance.GetCurrentStamina >= Stamina.Instance.MaxStamina -1 || staminaPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        Stamina.Instance.IncreaseStaminaWithPotion(staminaAmount);
        staminaPotions.DecreasePotionCount();
    }

}
