using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UseHPPotion : MonoBehaviour
{
    [SerializeField] float hpAmount;
    [SerializeField] HPPotions hPPotions;

    FirstPersonController firstPersonController;

    void Start() 
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
    }

    public void UseHPPotionFunc()
    {
        if(!GetComponentInParent<NetworkObject>().IsOwner) return;
        if(PlayerHP.Instance.GetCurrentHP >= PlayerHP.Instance.MaxHP -1 || hPPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        PlayerHP.Instance.IncreaseHP(hpAmount);
        hPPotions.DecreasePotionCount();
    }

}
