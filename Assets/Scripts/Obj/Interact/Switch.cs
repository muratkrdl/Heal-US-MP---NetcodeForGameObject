using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Switch : NetworkBehaviour, IInteractable
{
    [SerializeField] Animator moveObject;

    bool leverOn;

    void Start() 
    {
        leverOn = false;
    }

    public void AnimationEvent()
    {
        leverOn = true;
        SoundManager.Instance.PlaySound3D("Lever",transform.position);
        DestroyMoveObjServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void DestroyMoveObjServerRpc()
    {
        moveObject.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc(RequireOwnership = false)] void AnimationServerRpc()
    {
        AnimationClientRpc();
    }

    [ClientRpc] void AnimationClientRpc()
    {
        GetComponent<Animator>().SetTrigger("On");
        leverOn = true;
    }

    public void Interact()
    {
        if(leverOn) { return; }
        moveObject.SetTrigger("Move");
        AnimationServerRpc();
    }

}
