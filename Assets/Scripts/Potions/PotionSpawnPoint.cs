using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class PotionSpawnPoint : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] Vector2 spawnPotionsRange;

    const string HP_POTION_NAME = "HP Potion";
    const string MANA_POTION_NAME = "Mana Potion";
    const string STAMINA_POTION_NAME = "Stamina Potion";
    const string ICE_POTION_NAME = "Ice Potion";

    float randomSpawnRate;
    int randomChoosenPotion;

    string potionName;

    void Start() 
    {
        StartTimerForPotion();
    }

    public void StartTimerForPotion()
    {
        if(!PhotonNetwork.IsMasterClient) { return; }

        randomSpawnRate = Random.Range(spawnPotionsRange.x,spawnPotionsRange.y);
        randomChoosenPotion = Random.Range(0,4);
        StartCoroutine(SpawnPotions(randomSpawnRate,randomChoosenPotion));
        //PV.RPC(nameof(StartRoutine),RpcTarget.All,randomSpawnRate,randomChoosenPotion);
    }

    [PunRPC] void StartRoutine(float spawnRate,int choosenPotionIndex)
    {
        StartCoroutine(SpawnPotions(spawnRate,choosenPotionIndex));
    }

    IEnumerator SpawnPotions(float spawnRate,int choosenPotionIndex)
    {
        potionName = choosenPotionIndex switch
        {
            1 => HP_POTION_NAME,
            2 => MANA_POTION_NAME,
            3 => STAMINA_POTION_NAME,
            _ => ICE_POTION_NAME
        };
        yield return new WaitForSeconds(spawnRate);
        if(!PhotonNetwork.InLobby || !PhotonNetwork.InRoom) { yield return null; }
        var potion = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",potionName),transform.position,Quaternion.identity);
        potion.GetComponent<IPickUp>().SetParentTransformAsValue(transform);
        StopCoroutine(SpawnPotions(spawnRate,choosenPotionIndex));
    }

}
