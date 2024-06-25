using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlashObj : NetworkBehaviour
{
    [SerializeField] PlantType plantType;

    public PlantType GetPlantType
    {
        get
        {
            return plantType;
        }
    }

    public void KYS()
    {
        GetComponent<BoxCollider>().enabled = false;
        
        KYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        GetComponentInParent<NetworkObject>().Despawn();
    }

}
