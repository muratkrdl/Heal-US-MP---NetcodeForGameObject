using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItemUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lobbyNameText;
    [SerializeField] TextMeshProUGUI playersText;
    [SerializeField] Button joinButton;

    Lobby lobby;

    public void SetLobby(Lobby _lobby)
    {
        lobby = _lobby;
        lobbyNameText.text = lobby.Name;
        playersText.text = (lobby.MaxPlayers - lobby.AvailableSlots) + "/" + lobby.MaxPlayers;

        joinButton.onClick.AddListener( () =>
        {
            LobbyManager.Instance.JoinLobbyId(lobby.Id);
        });
    }

}
