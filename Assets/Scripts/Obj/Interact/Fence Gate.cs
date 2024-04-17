using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FenceGate : MonoBehaviour,IBoolInteractable
{
    [SerializeField] PhotonView PV;

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
        GetComponent<Animator>().SetTrigger("Open");
    }

    public void AnimationEvent()
    {
        SoundManager.Instance.PlaySound3D("Door",transform.position);
        gateOpen = true;
    }

}
