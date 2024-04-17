using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField createRoomNameField;

    [SerializeField] TextMeshProUGUI errorText;

    [Header("OnRoom")]
    [SerializeField] TextMeshProUGUI roonNameText;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Transform playerListContent;
    [SerializeField] PlayerListItem playerListItemPrefab;

    [Header("Find Room")]
    [SerializeField] Transform roomListContent;
    [SerializeField] RoomListItem roomListItemPrefab;

    PhotonView PV;

    void Awake() 
    {
        Instance = this;
        PV = GetComponent<PhotonView>();
    }

    void Start() 
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            MainMenuManager.Instance.OpenMenu("loading");
        }    
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        MainMenuManager.Instance.OpenMenu("mainmenu");
    }

    public void MultiPlayerButtonEvent()
    {
        Debug.Log("Connecting to Master");
        MainMenuManager.Instance.OpenMenu("loading");
        if(!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else if(!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        else
        {
            MainMenuManager.Instance.OpenMenu("title");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to lobby");
        MainMenuManager.Instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(createRoomNameField.text))
        {
            Debug.Log("Invalid Room Name");
            return;
        }

        PhotonNetwork.CreateRoom(createRoomNameField.text);
        MainMenuManager.Instance.OpenMenu("loading");
        createRoomNameField.text = "";
    }

    public override void OnJoinedRoom()
    {
        MainMenuManager.Instance.OpenMenu("room");
        roonNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform item in playerListContent)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab,playerListContent).SetPlayer(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed" + message;
        MainMenuManager.Instance.OpenMenu("error");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MainMenuManager.Instance.OpenMenu("loading");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MainMenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MainMenuManager.Instance.OpenMenu("title");
    }

    public void StartGame()
    {
        PV.RPC(nameof(RPC_StartGame), RpcTarget.All);
    }

    [PunRPC] void RPC_StartGame()
    {
        MainMenuManager.Instance.SetFade();
        
        if(PhotonNetwork.IsMasterClient)
            Invoke(nameof(LoadGameLevel), 1.5f);
    }

    void LoadGameLevel()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform item in roomListContent)
        {
            Destroy(transform.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab,roomListContent).SetRoom(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab,playerListContent).SetPlayer(newPlayer);
    }

}
