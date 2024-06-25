using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Key : NetworkBehaviour, IBoolInteractable
{
    public void Interact(Interact interact)
    {
        interact.HasKey = true;
        SoundManager.Instance.PlaySound3D("Get Key", transform.position);
        DestroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void DestroyServerRpc()
    {
        DestroyClientRpc();
    }

    [ClientRpc] void DestroyClientRpc()
    {
        gameObject.SetActive(false);
    }

}
