using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FenceGate : NetworkBehaviour, IBoolInteractable
{
    bool gateOpen = false;

    public void Interact(Interact interact)
    {
        if(gateOpen) { return; }
        if(!interact.HasKey)
        {
            Manager.Instance.SpawnFloatingText(transform.position,"YOU NEED KEY",Color.red);
            return;
        }

        Interact();
    }

    void Interact()
    {
        AnimationServerRpc();
    }

    public void AnimationEvent()
    {
        SoundManager.Instance.PlaySound3D("Door",transform.position);
        gateOpen = true;
    }

    [ServerRpc(RequireOwnership = false)] void AnimationServerRpc()
    {
        AnimationClientRpc();
    }

    [ClientRpc] void AnimationClientRpc()
    {
        GetComponent<Animator>().SetTrigger("Open");
        gateOpen = true;
    }

}
