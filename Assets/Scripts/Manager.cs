using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : NetworkBehaviour
{
    public static Manager Instance;

	[SerializeField] GameTimer gameTimer;
	[SerializeField] GameObject waitingOtherPlayersCanvas;

    [SerializeField] float loadDelay;

    [SerializeField] Camera cam;
    [SerializeField] Image blackImage;

    [SerializeField] GameObject floatingTextPrefab;

    [SerializeField] EscMenu escMenu;

    int villagerDeadCounter = 0;

    string loadSceneName = "";

    int readyCounter = 0;

    bool requiredTypeHasDisconnected;

    int maxPlayers;

    
	bool canStart = false;
	int canStartCounter;
	int neededAmountForStart;


    public bool GetCanStart
    {
        get
        {
            return canStart;
        }
    }
 
    public bool RequiredTypeHasDisconnected
    {
        get
        {
            return requiredTypeHasDisconnected;
        }
        set
        {
            requiredTypeHasDisconnected = value;
        }
    }

    public EscMenu GetEscMenu
    {
        get
        {
            return escMenu;
        }
    }

    public void ClientConnected()
    {
        Invoke(nameof(IncreaseReadyPlayerServerRpc), 1);
    }

    [ServerRpc(RequireOwnership = false)] void IncreaseReadyPlayerServerRpc()
	{
		canStartCounter++;
		if(canStartCounter >= neededAmountForStart)
		{
			GameCanStartClientRpc();
			if(gameTimer != null)
			{
				gameTimer.StartGameTimer();
			}
		}
	}

	[ClientRpc] void GameCanStartClientRpc()
	{
		canStart = true;
		waitingOtherPlayersCanvas.SetActive(false);
	}

    public void SetCamera(bool value)
    {
        cam.gameObject.SetActive(value);
    }

    void Awake()
    {
        Instance = this;
    }

    void Start() 
    {
        Application.quitting += Application_Quitting;
        maxPlayers = LobbyManager.Instance.GetCurrentPlayersAmount;
        neededAmountForStart = maxPlayers;
    }

    public override void OnDestroy() 
    {
        Application.quitting -= Application_Quitting;
    }

    public void Application_Quitting()
    {
        if(!Instance.RequiredTypeHasDisconnected)
        {
            var type = escMenu.FirstPersonController.HumanType;
            if(type == HumanType.patrick || type == HumanType.doctor)
            {
                RequiredTypeHasDisconnectedServerRpc();
            }
        }
        if(LobbyManager.Instance != null)
        {
            LobbyManager.Instance.KYS();
        }
        if(NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)] public void RequiredTypeHasDisconnectedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        RequiredTypeHasDisconnectedClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc] void RequiredTypeHasDisconnectedClientRpc(ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId) return;
        Instance.SetCamera(true);
        Instance.RequiredTypeHasDisconnected = true;
        escMenu.SetRequiredTypeDisconnectedPanel(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void SpawnFloatingText(Vector3 pos,string str,Color color)
    {
        SpawnFloatingTextServerRpc(pos, str);
    }

    [ServerRpc(RequireOwnership = false)] void SpawnFloatingTextServerRpc(Vector3 pos,string str)
    {
        SpawnFloatingTextClientRpc(pos, str);
    }

    [ClientRpc] void SpawnFloatingTextClientRpc(Vector3 pos,string str)
    {
        var text = Instantiate(floatingTextPrefab,pos,Quaternion.identity);
        text.GetComponent<FloatingText>().SetTextValues(str, Color.red);
    }

    public void PatrickLoseGame()
    {
        ChangeScene("PatrickLose");
    }

    public void PatrickWinGame()
    {
        ChangeScene("PatrickWin");
    }

    void ChangeScene(string str)
    {
        ChangeSceneServerRpc(str);
    }

    [ServerRpc(RequireOwnership = false)] void ChangeSceneServerRpc(string str)
    {
        ChangeSceneClientRpc(str);
        
        Invoke(nameof(LoadLevelClientRpc), loadDelay);
    }

    [ClientRpc] void ChangeSceneClientRpc(string str)
    {
        loadSceneName = str;
        blackImage.gameObject.SetActive(true);
        blackImage.GetComponent<Animator>().SetTrigger("Fade");
    }

    [ClientRpc] void LoadLevelClientRpc()
    {
        if(IsClient)
        {
            LoadingSceneServerRpc();
        }

        SceneManager.LoadScene(loadSceneName);
    }

    [ServerRpc(RequireOwnership = false)] void LoadingSceneServerRpc()
    {
        readyCounter += 1;

        if(readyCounter >= LobbyManager.Instance.GetMaxPlayers -1)
        {
            SceneManager.LoadScene(loadSceneName);
        }
    }

    public void CheckPatrickLose()
    {
        CheckPatrickLoseServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void CheckPatrickLoseServerRpc()
    {
        villagerDeadCounter++;
        if(villagerDeadCounter >= maxPlayers -2)
        {
            PatrickLoseGame();
        }
    }

}
