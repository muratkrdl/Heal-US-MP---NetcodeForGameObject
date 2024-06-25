using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    [SerializeField] GameObject[] spawnableChar;

    EventHandler<OnCharacterSpawned> characterSpawned;
    public class OnCharacterSpawned : EventArgs
    {
        public ulong clientId;
    }

    public PlayerData playerData;

    int myCharacterClassIndex = -1;

    void Start() 
    {
        if(!IsOwner) return;

        Invoke(nameof(SetDatas), 2);
        characterSpawned += MyCharacterSpawned;
    }

    public override void OnDestroy() 
    {
        characterSpawned -= MyCharacterSpawned;
    }

    void MyCharacterSpawned(object sender, OnCharacterSpawned e)
    {
        if(e.clientId == OwnerClientId)
        {
            var players = FindObjectsOfType<FirstPersonController>();
            foreach (var item in players)
            {
                if(item.GetComponent<NetworkObject>().IsOwner)
                {
                    item.HumanType = myCharacterClassIndex switch
                    {
                        1 => HumanType.patrick,
                        2 => HumanType.doctor,
                        _ => HumanType.villager
                    };

                    item.gameObject.layer = LayerMask.NameToLayer("MyChar");
                    break;
                }
                else
                {
                    continue;
                }
            }

            Manager.Instance.SetCamera(false);
        }
    }

    void SetDatas()
    {
        playerData = LobbyManager.Instance.GetPlayerData();

        myCharacterClassIndex = playerData.characterIndex;

        SpawnMyCharacterServerRpc(myCharacterClassIndex);
    }

    [ServerRpc(RequireOwnership = false)] void SpawnMyCharacterServerRpc(int index, ServerRpcParams serverRpcParams = default)
    {
        SpawnMyCharacter(index, serverRpcParams.Receive.SenderClientId);
    }

    void SpawnMyCharacter(int index, ulong clientId)
    {
        GameObject spawnChar = index switch
        {
            1 => spawnableChar[0],
            2 => spawnableChar[1],
            _ => spawnableChar[2]
        };

        var spawnObj = Instantiate(spawnChar);
        spawnObj.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);

        CharacterSpawnedClientRpc(clientId);
    }

    [ClientRpc] void CharacterSpawnedClientRpc(ulong _clientId)
    {
        characterSpawned?.Invoke(this, new()
        {
            clientId = _clientId
        });
    }

}
