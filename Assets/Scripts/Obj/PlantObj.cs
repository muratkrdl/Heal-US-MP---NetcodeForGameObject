using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlantObj : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    public void KYS()
    {
        PV.RPC(nameof(RPC_KYS), RpcTarget.All);
    }

    [PunRPC] void RPC_KYS()
    {
        Destroy(gameObject);
    }

}
