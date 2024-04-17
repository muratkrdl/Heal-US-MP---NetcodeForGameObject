using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FlashObj : MonoBehaviour
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
        
        if(GetComponent<PhotonView>().IsMine)
            PhotonNetwork.Destroy(transform.parent.gameObject);
    }

}
