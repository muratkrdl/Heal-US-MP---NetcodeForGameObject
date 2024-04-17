using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Key : MonoBehaviour,IBoolInteractable
{
    [SerializeField] PhotonView PV;

    public void Interact(Interact interact)
    {
        interact.HasKey = true;
        SoundManager.Instance.PlaySound3D("Get Key", transform.position);
        PV.RPC(nameof(DestroyObject),RpcTarget.All);
    }

    [PunRPC] void DestroyObject()
    {
        Destroy(gameObject);
    }

}
