using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    [SerializeField] PhotonView PV;

    PlayerManager[] playerManagers;

    bool hasPatrick;
    bool hasDoctor;

    public PlayerManager[] PlayerManagers
    {
        get
        {
            return playerManagers;
        }
        set
        {
            playerManagers = value;
        }
    }

    void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 2)
        {
            var playerManager = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","PlayerManager"),Vector3.zero,Quaternion.identity);

            Invoke(nameof(FindAllPlayerManagers), 2);
            if(PhotonNetwork.IsMasterClient)
            {
                Invoke(nameof(SetHumanType), 1);
            }
        }
    }

    void FindAllPlayerManagers()
    {
        playerManagers = FindObjectsOfType<PlayerManager>();
    }

    void SetHumanType()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Hashtable playerProps = new()
            {
                { "Type", SetHumanTypeHashtable() }
            };
            if(player.CustomProperties.ContainsKey("Type"))
            {
                player.CustomProperties.Remove("Type");
            }
            player.SetCustomProperties(playerProps);
        }
    }

    int SetHumanTypeHashtable()
    {
        if(!hasPatrick)
        {
            hasPatrick = true;
            return 0;
        }
        else if(!hasDoctor)
        {
            hasDoctor = true;
            return 1;
        }
        return 2;
    }

}
