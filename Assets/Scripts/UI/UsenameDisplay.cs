using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UsenameDisplay : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    
    void Start()
    {
        if(IsOwner)
        {
            gameObject.SetActive(false);
        }

        nameText.text = LobbyManager.Instance.GetPlayerDataFromClientId(OwnerClientId).playerName.ToString();
    }

}
