using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlantObj : NetworkBehaviour
{
    bool collectable = true;

    public bool GetCollectable
    {
        get
        {
            return collectable;
        }
    }

    public void KYS()
    {
        if(!collectable) return;
        collectable = false;
        KYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        KYSClientRpc();
    }

    [ClientRpc] void KYSClientRpc()
    {
        gameObject.SetActive(false);
    }

}
