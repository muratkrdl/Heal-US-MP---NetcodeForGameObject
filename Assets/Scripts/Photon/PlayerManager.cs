using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum HumanType
{
    patrick,
    doctor,
    villager
}

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PhotonView PV;

    GameObject controller;

    HumanType state;

    FirstPersonController fpsPlayer;

    bool isDead = false;
    const string PATRICK_PREFAB_NAME = "Patrick";
    const string DOCTOR_PREFAB_NAME = "Doctor";
    const string VILLAGER_PREFAB_NAME = "Villager 0";

    public Player GetPhotonPlayer
    {
        get
        {
            return PhotonNetwork.LocalPlayer;
        }
    }

    public FirstPersonController GetFPSPlayer
    {
        get
        {
            return fpsPlayer;
        }
    }

    public HumanType GetHumanType
    {
        get
        {
            return state;
        }
    }

    
	public bool GetIsDead
    {
        get
        {
            return isDead;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("Changed Something");
        if(!PV.IsMine) { return; }
        if(targetPlayer == GetPhotonPlayer)
        {
            if(changedProps.ContainsKey("Type"))
            {
                if(controller != null) { PhotonNetwork.Destroy(controller); }
                int type = (int)changedProps["Type"];

                state = type switch
                {
                    0 => HumanType.patrick, 
                    1 => HumanType.doctor,
                    _ => HumanType.villager
                };

                CreateController();
            }
            else if(changedProps.ContainsKey("Dead"))
            {
                isDead = (bool)changedProps["Dead"];
            }
        }
    }

    void CreateController()
    {
        string controllerName = state switch
        {
            HumanType.patrick => PATRICK_PREFAB_NAME,
            HumanType.doctor => DOCTOR_PREFAB_NAME,
            _ => VILLAGER_PREFAB_NAME
        };
        Transform spawnTransform = state switch
        {
            HumanType.patrick => SpawnManager.Instance.GetPatrickSpawnpoint,
            HumanType.doctor => SpawnManager.Instance.GetDoctorSpawnpoint,
            _ => SpawnManager.Instance.GetVillagerSpawnPoint()
        };

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",controllerName), spawnTransform.position, spawnTransform.rotation,0,new object[] { PV.ViewID } );
        fpsPlayer = controller.GetComponent<FirstPersonController>();
        controller.layer = LayerMask.NameToLayer("MyChar");
        Manager.Instance.SetPlayerManager = this;
        Manager.Instance.SetCamera(false);
        controller.GetComponentInChildren<EscMenu>().SetUnFade();
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
    }

    public static PlayerManager FindPlayer(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
    
}
