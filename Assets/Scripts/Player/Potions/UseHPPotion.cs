using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UseHPPotion : MonoBehaviour
{
    [SerializeField] float hpAmount;
    [SerializeField] HPPotions hPPotions;

    PlayerManager playerManager;
    FirstPersonController firstPersonController;

    void Start() 
    {
        firstPersonController = GetComponentInParent<FirstPersonController>();
        playerManager = firstPersonController.GetPlayerManager;
    }

    public void UseHPPotionFunc()
    {
        if(PlayerHP.Instance.GetCurrentHP >= PlayerHP.Instance.MaxHP -1 || hPPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        PlayerHP.Instance.IncreaseHP(hpAmount);
        hPPotions.DecreasePotionCount();
    }

}
