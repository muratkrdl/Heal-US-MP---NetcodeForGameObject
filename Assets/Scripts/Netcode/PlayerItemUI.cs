using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;

    [SerializeField] Button kickButton;

    Player player;

    public void SetPlayer(Player _player, string senderPlayerId, bool isHost)
    {
        player = _player;
        playerNameText.text = player.Data["PlayerName"].Value;
        if(isHost && senderPlayerId != player.Id)
        {
            kickButton.onClick.AddListener(() => LobbyManager.Instance.KickPlayer(player.Id));
            kickButton.gameObject.SetActive(true);
        }
    }

}
