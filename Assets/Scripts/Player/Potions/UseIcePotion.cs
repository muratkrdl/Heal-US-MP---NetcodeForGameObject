using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class UseIcePotion : NetworkBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Transform mouseLookPos;

    [SerializeField] GameObject iceObjPrefab;

    [SerializeField] Transform throwPosition;
    [SerializeField] Transform player;
    [SerializeField] float throwSpeed;
    [SerializeField] float throwUpForce;

    [SerializeField] IcePotions icePotions;

    public void UseIcePotionAsAbility()
    {
        var direction = (mouseLookPos.position - cam.transform.position).normalized;
        Vector3 throwForce = direction * throwSpeed + transform.up * throwUpForce;
        icePotions.DecreasePotionCount();
        ThrowIcePotionServerRpc(throwForce, throwPosition.position);
    }

    [ServerRpc(RequireOwnership = false)] void ThrowIcePotionServerRpc(Vector3 direction, Vector3 pos)
    {
        var ice = Instantiate(iceObjPrefab, pos, Quaternion.identity); 
        ice.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
        ice.GetComponent<AbilityIcePotion>().SetPlayer = player;
        ice.GetComponent<AbilityIcePotion>().ThrowPotion(direction);
    }

}
