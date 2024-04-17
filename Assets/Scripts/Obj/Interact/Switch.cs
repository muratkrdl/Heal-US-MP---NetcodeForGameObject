using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Switch : MonoBehaviour,IInteractable
{
    [SerializeField] PhotonView PV;
    [SerializeField] GameObject moveObject;

    bool leverOn;

    void Start() 
    {
        leverOn = false;
    }

    public void AnimationEvent()
    {
        leverOn = true;
        SoundManager.Instance.PlaySound3D("Lever",transform.position);
        PV.RPC(nameof(DestroyMoveObject),RpcTarget.All);
    }

    [PunRPC] void DestroyMoveObject()
    {
        Destroy(moveObject);
    }

    public void Interact()
    {
        if(leverOn) { return; }
        PV.RPC(nameof(RPC_Interact),RpcTarget.All);
    }

    [PunRPC] void RPC_Interact()
    {
        GetComponent<Animator>().SetTrigger("On");
    }

}
