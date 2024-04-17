using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class UseManaPotion : MonoBehaviour
{
    [SerializeField] float manaAmount;
    [SerializeField] ManaPotions manaPotions;

    PlayerManager playerManager;

    void Start() 
    {
        playerManager = GetComponentInParent<FirstPersonController>().GetPlayerManager;    
    }

    public void UseManaPotionFunc()
    {
        if(Mana.Instance.GetCurrentMana >= Mana.Instance.MaxMana -1 || manaPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        Mana.Instance.IncreaseMana(manaAmount);
        manaPotions.DecreasePotionCount();
    }

}
