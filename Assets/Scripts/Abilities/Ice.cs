using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Ice : NetworkBehaviour
{
    [SerializeField] int lifeTime;
    [SerializeField] float slowTime;
    [SerializeField] float slowAmount;

    [SerializeField] ParticleSystem iceFX;
    [SerializeField] BoxCollider boxCollider;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        Invoke(nameof(KYS),lifeTime);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<FirstPersonController>(out var player))
        {
            player.MoveSpeed = player.GetInitialMoveSpeed / slowAmount;
            player.SprintSpeed = player.GetInitialSprintSpeed / slowAmount;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<FirstPersonController>(out var player))
        {
            player.MoveSpeed = player.GetInitialMoveSpeed;
            player.SprintSpeed = player.GetInitialSprintSpeed;
        }
    }

    void KYS()
    {
        KYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        KYSClientRpc();
    }

    [ClientRpc] void KYSClientRpc()
    {
        iceFX.Stop();
        boxCollider.size = Vector3.zero;
        if(IsServer)
        {
            Invoke(nameof(DeSpawnObj), slowTime + 5);
        }
    }

    void DeSpawnObj()
    {
        GetComponent<NetworkObject>().Despawn();
    }

}
