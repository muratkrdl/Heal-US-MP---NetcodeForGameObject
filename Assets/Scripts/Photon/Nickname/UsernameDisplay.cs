using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPV;
    [SerializeField] TextMeshProUGUI text;

    void Start() 
    {
        if(playerPV.IsMine)
        {
            gameObject.SetActive(false);
        }

        text.text = playerPV.Owner.NickName;
    }

}
