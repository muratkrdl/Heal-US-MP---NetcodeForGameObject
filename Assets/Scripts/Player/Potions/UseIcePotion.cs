using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UIElements;

public class UseIcePotion : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform mouseLookPos;

    [SerializeField] Transform throwPosition;
    [SerializeField] Transform player;
    [SerializeField] float throwSpeed;
    [SerializeField] float throwUpForce;

    [SerializeField] int slowAmount;
    [SerializeField] IcePotions icePotions;

    public int SlowAmount
    {
        get
        {
            return slowAmount;
        }
        set
        {
            slowAmount = value;
        }
    }

    public void UseIcePotionAsAbility()
    {
        var direction = (mouseLookPos.position - cam.transform.position).normalized;
        Vector3 throwForce = direction * throwSpeed + transform.up * throwUpForce;
        icePotions.DecreasePotionCount();
        ThrowIcePotion(throwForce);
    }

    void ThrowIcePotion(Vector3 direction)
    {
        var ice = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","Ability Ice Potion"),throwPosition.position,cam.transform.rotation);
        ice.GetComponent<AbilityIcePotion>().SetPlayer = player;
        ice.GetComponent<AbilityIcePotion>().SetSlowAmount = slowAmount;
        ice.GetComponent<AbilityIcePotion>().ThrowPotion(direction);
    }

}
