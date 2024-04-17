using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI playerNameText;

    Player player;

    public Player GetPlayer
    {
        get
        {
            return player;
        }
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
        playerNameText.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

}
