using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    const string KEY_RELAY_CODE = "RelayCode";
    const string IS_GAME_STARTED = "IsGameStarted";
    const string IS_GAME_STARTED_FALSE = "false";
    const string IS_GAME_STARTED_TRUE = "true";

    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinFailed;

    [Header("Main Menu Panel")]
    [SerializeField] GameObject lobbyInfoPrefab;
    [SerializeField] Transform lobbiesInfoContent;

    [SerializeField] TextMeshProUGUI playerNameText;

    [Space(10)]
    [Header("Create Room Panel")]
    [SerializeField] TextMeshProUGUI inputRoomNameText;
    [SerializeField] TextMeshProUGUI isPrivateButtonText;
    [SerializeField] TextMeshProUGUI maxPlayersText;

    [SerializeField] Button decreaseButton;
    [SerializeField] Button increaseButton;
    [SerializeField] int minPlayer;
    [SerializeField] int maxPlayer;

    [Space(10)]
    [Header("Room Panel")]
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] TextMeshProUGUI roomCodeText;
    [SerializeField] Transform playerInfoContent;
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Button leaveRoomButton;
    [SerializeField] Button startGameButton;

    [Space(10)]
    [Header("Join Room With Code")]
    [SerializeField] TextMeshProUGUI joinRoomWithCodeText;
    [SerializeField] Button joinWithCodeButton;

    string playerName;

    Lobby currentLobby;

    int currentMaxPlayersIndex = 4;
    bool isPrivate = false;

    float heartBeatTimer = 15;
    float roomUpdateTimer = 1.1f;
    float updateLobbiesTimer = 1.1f;

    float canDeleteCounter = 0;

    NetworkList<PlayerData> playerDataNetworkList;

    bool hasPatrick = false;
    bool hasDoctor = false;

    int currentPlayersAmount;

    public int GetCurrentPlayersAmount
    {
        get
        {
            return currentPlayersAmount;
        }
    }

    public int GetMaxPlayers
    {
        get
        {
            return currentMaxPlayersIndex;
        }
    }

    void Awake() 
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        hasPatrick = false;
        hasDoctor = false;

        playerDataNetworkList = new();
    }

    public override void OnDestroy() 
    {
        try
        {
            if(currentLobby != null)
            {
                if(IsMyHost())
                {
                    Lobbies.Instance.DeleteLobbyAsync(currentLobby.Id);
                }
                else
                {
                    Lobbies.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                }
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void InitializeUnityAuthentication()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            MainMenuManager.Instance.OpenMenu("loading");

            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0,1000).ToString());
         
            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                //
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            MainMenuManager.Instance.OpenMenu("username");
        }
        else
        {
            MainMenuManager.Instance.OpenMenu("username");
        }
    }

    void Update() 
    {
        if(SceneManager.GetActiveScene().buildIndex != 1) return;
        
        if(currentLobby != null)
        {
            HandleRoomUpdate();
            HandleHeartBeat();
        }

        if(!IsInLobby())
        {
            HandleListLobbies();
        }
    }

    async void HandleHeartBeat()
    {
        if(IsMyHost())
        {
            heartBeatTimer -= Time.deltaTime;
            if(heartBeatTimer <= 0)
            {
                heartBeatTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }

#region ListLobbies
    void HandleListLobbies()
    {
        if(currentLobby == null &&
        UnityServices.State == ServicesInitializationState.Initialized &&
        AuthenticationService.Instance.IsSignedIn &&
        SceneManager.GetActiveScene().name == "MainMenu")
        {
            updateLobbiesTimer -= Time.deltaTime;
            if(updateLobbiesTimer <= 0)
            {
                updateLobbiesTimer = 3f;
                ListPublicLobbies();
            }
        }
    }
    async void ListPublicLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Filters = new List<QueryFilter>()
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    new(QueryFilter.FieldOptions.S1, IS_GAME_STARTED_FALSE, QueryFilter.OpOptions.EQ),
                    new(QueryFilter.FieldOptions.S2, " ", QueryFilter.OpOptions.NE)
                }
            };
            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            VisualizeLobbiesList(response.Results);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    void VisualizeLobbiesList(List<Lobby> _publicLobbies)
    {
        if(SceneManager.GetActiveScene().buildIndex != 1) return;
        foreach(Transform item in lobbiesInfoContent)
        {
            if (item == lobbyInfoPrefab) continue;
            Destroy(item.gameObject);
        }
        foreach(Lobby item in _publicLobbies)
        {
            if(item.IsPrivate) { continue; }
            var newLobbyInfo = Instantiate(lobbyInfoPrefab, lobbiesInfoContent);
            newLobbyInfo.GetComponent<LobbyItemUI>().SetLobby(item);
        }
    }

#endregion

#region RoomDetail
    async void HandleRoomUpdate()
    {
        if(currentLobby != null && IsInLobby())
        {
            try
            {
                roomUpdateTimer -= Time.deltaTime;
                if(roomUpdateTimer <= 0)
                {
                    roomUpdateTimer = 1.1f;
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                    currentLobby = lobby;
                    VisualizeRoomDetails();

                    if(currentLobby == null) return;
                    if(currentLobby.Data[IS_GAME_STARTED].Value == IS_GAME_STARTED_TRUE && !IsMyHost())
                    {
                        EnterGameForClients();
                    }
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
                if(e.Reason == LobbyExceptionReason.LobbyNotFound || e.Reason == LobbyExceptionReason.Forbidden)
                {
                    currentLobby = null;
                    ExitRoom();
                }
            }
        }
    }
    void EnterRoom()
    {
        MainMenuManager.Instance.OpenMenu("room");
        roomNameText.text = currentLobby.Name;
        roomCodeText.text = currentLobby.LobbyCode;

        VisualizeRoomDetails();
    }
    void VisualizeRoomDetails()
    {
        if(currentLobby == null || SceneManager.GetActiveScene().buildIndex != 1) return;
        foreach (Transform item in playerInfoContent)
        {
            Destroy(item.gameObject);
        }
        if(IsInLobby())
        {
            foreach(Player player in currentLobby.Players)
            {
                var playerInfo = Instantiate(playerInfoPrefab, playerInfoContent);
                playerInfo.GetComponent<PlayerItemUI>().SetPlayer(player, AuthenticationService.Instance.PlayerId, IsMyHost());
            }

            if(IsMyHost())
            {
                startGameButton.gameObject.SetActive(true);
            }
            else
            {
                startGameButton.gameObject.SetActive(false);
            }
        }
        else
        {
            ExitRoom();
        }
    }
#endregion

#region Relay
    async Task<Allocation> AllocateRelay(int _maxPlayers)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_maxPlayers -1);
            return allocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        } 
    }

    async Task<string> GetRelayJoincode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        } 
    }

    async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
#endregion

#region JoinLobbyLeaveLobby.etc.
    public async void CreateLobby()
    {
        MainMenuManager.Instance.OpenMenu("loading");
        try
        {
            string roomName = inputRoomNameText.text;
            int maxPlayers = currentMaxPlayersIndex;
            bool privateState = isPrivate;

            CreateLobbyOptions createLobbyOptions = new()
            {
                IsPrivate = privateState,
                Player = GetPlayer(),
                Data = new()
                {
                    { IS_GAME_STARTED, new(DataObject.VisibilityOptions.Member, IS_GAME_STARTED_FALSE, DataObject.IndexOptions.S1) },
                    { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member, " ", DataObject.IndexOptions.S2) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync(roomName, maxPlayers, createLobbyOptions);
            
            Allocation allocation = await AllocateRelay(maxPlayers);
            string relayJoinCode = await GetRelayJoincode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions 
            {
                Data = new Dictionary<string, DataObject>
                {
                    { KEY_RELAY_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode, DataObject.IndexOptions.S2) }
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            EnterRoom();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyId(string lobbyId)
    {
        MainMenuManager.Instance.OpenMenu("loading");
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new()
            {
                Player = GetPlayer()
            };
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);

            string relayJoinCode = currentLobby.Data[KEY_RELAY_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(joinAllocation, "dtls"));

            EnterRoom();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void JoinLobbyWithCode()
    {
        MainMenuManager.Instance.OpenMenu("loading");
        string code = joinRoomWithCodeText.text;
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new()
            {
                Player = GetPlayer()
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);

            string relayJoinCode = currentLobby.Data[KEY_RELAY_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new(joinAllocation, "dtls"));

            EnterRoom();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void Deletelobby()
    {
        if(currentLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

                currentLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveRoom()
    {
        MainMenuManager.Instance.OpenMenu("loading");
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
            currentLobby = null;
            ExitRoom();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    void ExitRoom()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            MainMenuManager.Instance.OpenMenu("mainmenu");
        }
        else
        {
            MainMenuManager.Instance.OpenMenu("title");
        }
    }

    public async void KickPlayer(string _playerId)
    {
        if(IsMyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, _playerId);
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

#endregion

    public void StartGame()
    {
        if(currentLobby != null)
        {
            try
            {
                EnterGame();
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    async void EnterGame()
    {
        UpdateLobbyOptions updateLobbyOptions = new()
        {
            Data = new()
            {
                { IS_GAME_STARTED, new(DataObject.VisibilityOptions.Member, IS_GAME_STARTED_TRUE, DataObject.IndexOptions.S1) }
            }
        };
        currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateLobbyOptions);

        MainMenuManager.Instance.SetFade();
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManagerOnClientConnectedCallback;
        NetworkManager.Singleton.StartHost();
        startGameButton.interactable = false;

        currentPlayersAmount = currentLobby.MaxPlayers - currentLobby.AvailableSlots;

        Invoke(nameof(LoadSceneMainGameScene), 1);
    }

    void LoadSceneMainGameScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("MultiplayerGame", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void EnterGameForClients()
    {
        CanDeleteLobbyServerRpc();
        MainMenuManager.Instance.SetFade();
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_On_Client_ConnectedCallback;
        NetworkManager.Singleton.StartClient();
        currentLobby = null;
    }

    void NetworkManagerOnClientConnectedCallback(ulong clientId)
    {
        int charIndex;
        if(!hasPatrick)
        {
            charIndex = 1;
            hasPatrick = true;
        }
        else if(!hasDoctor)
        {
            charIndex = 2;
            hasDoctor = true;
        }
        else
        {
            charIndex = 3;
        }

        playerDataNetworkList.Add(new PlayerData()
        {
            playerName = GetPlayerName(),
            clientId = clientId,
            characterIndex = charIndex,
        });

        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    void NetworkManager_On_Client_ConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    public string GetPlayerName() 
    {
        return playerName;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId) 
    {
        for (int i=0; i< playerDataNetworkList.Count; i++) 
        {
            if (playerDataNetworkList[i].clientId == clientId) 
            {
                return i;
            }
        }
        return -1;
    }

    [ServerRpc(RequireOwnership = false)] void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)] void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)] void SetPlayerClassServerRpc(int index, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.characterIndex = index;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId) 
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId) 
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)] void CanDeleteLobbyServerRpc()
    {
        canDeleteCounter++;
        if(canDeleteCounter >= currentLobby.MaxPlayers - currentLobby.AvailableSlots -1)
        {
            Deletelobby();
        }
    }

#region Helper
    bool IsInLobby()
    {
        if(currentLobby == null)
        {
            return false;
        }
        foreach (Player _Player in currentLobby.Players)
        {
            if(_Player.Id == AuthenticationService.Instance.PlayerId)
            {
                return true;
            }
        }
        currentLobby = null;
        return false;
    }

    bool IsMyHost()
    {
        return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    Player GetPlayer()
    {
        string playerName = PlayerPrefs.GetString("PlayerName");

        Player player = new()
        {
            Data = new()
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };

        return player;
    }

#endregion

    public void SaveName()
    {
        if(string.IsNullOrEmpty(playerNameText.text))
        {
            playerName = "Player" + UnityEngine.Random.Range(0,9999).ToString("0000");
        }
        else
        {
            playerName = playerNameText.text;
        }
        PlayerPrefs.SetString("PlayerName", playerName);
        MainMenuManager.Instance.OpenMenu("title");
    }

    public void ChangeMaxPlayers(bool increase)
    {
        if(increase)
        {
            currentMaxPlayersIndex++;
            maxPlayersText.text = currentMaxPlayersIndex.ToString();
            if(!decreaseButton.interactable) { decreaseButton.interactable = true; }
            if(currentMaxPlayersIndex >= maxPlayer)
            {
                increaseButton.interactable = false;
            }
        }
        else
        {
            currentMaxPlayersIndex--;
            maxPlayersText.text = currentMaxPlayersIndex.ToString();
            if(!increaseButton.interactable) { increaseButton.interactable = true; }
            if(currentMaxPlayersIndex <= minPlayer)
            {
                decreaseButton.interactable = false;
            }
        } 
    }

    public void IsPrivateButtonEvent()
    {
        isPrivate = !isPrivate;
        if(isPrivate)
        {
            isPrivateButtonText.text = "Private";
        }
        else
        {
            isPrivateButtonText.text = "Public";
        }
    }

    public void KYS()
    {
        if(IsInLobby())
        {
            if(IsMyHost())
            {
                Deletelobby();
                DeleteLobbyClientRpc();
            }
            else
            {
                LeaveRoom();
            }
        }

        Destroy(gameObject);
    }

    [ClientRpc] void DeleteLobbyClientRpc()
    {
        currentLobby = null;
    }

}
