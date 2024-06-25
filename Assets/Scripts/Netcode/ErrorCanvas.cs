using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ErrorCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI errorText;

    void Start() 
    {
        LobbyManager.Instance.OnJoinFailed += LobbyManager_OnFailedToJoinGame;
        LobbyManager.Instance.OnCreateLobbyFailed += LobbyManager_OnCreateLobbyFailed;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnected;
    }

    void NetworkManager_OnClientDisconnected(ulong obj)
    {
        MainMenuManager.Instance.OpenMenu("error");

        if (NetworkManager.Singleton.DisconnectReason == "") 
        {
            errorText.text = "Failed to connect";
        } 
        else 
        {
            errorText.text = NetworkManager.Singleton.DisconnectReason;
        }
    }

    void LobbyManager_OnCreateLobbyFailed(object sender, EventArgs e)
    {
        MainMenuManager.Instance.OpenMenu("error");
        errorText.text = "Failed to create Lobby!";
    }

    void LobbyManager_OnQuickJoinFailed(object sender, EventArgs e)
    {
        MainMenuManager.Instance.OpenMenu("error");
        errorText.text = "Could not find a Lobby to Quick Join!";
    }

    void LobbyManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        MainMenuManager.Instance.OpenMenu("error");
        errorText.text = "Failed to join Lobby!";
    }

    void OnDestroy() 
    {
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnected;
        }
        if(LobbyManager.Instance != null)
        {
            LobbyManager.Instance.OnCreateLobbyFailed -= LobbyManager_OnCreateLobbyFailed;
            LobbyManager.Instance.OnJoinFailed -= LobbyManager_OnFailedToJoinGame;
        }
    }

}
