using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UseManaPotion : MonoBehaviour
{
    [SerializeField] float manaAmount;
    [SerializeField] ManaPotions manaPotions;

    public void UseManaPotionFunc()
    {
        if(!GetComponentInParent<NetworkObject>().IsOwner) return;
        if(Mana.Instance.GetCurrentMana >= Mana.Instance.MaxMana -1 || manaPotions.GetCurrentCount <= 0) { return; }
        SoundManager.Instance.PlaySound3D("Drink",transform.position);
        Mana.Instance.IncreaseMana(manaAmount);
        manaPotions.DecreasePotionCount();
    }

}
