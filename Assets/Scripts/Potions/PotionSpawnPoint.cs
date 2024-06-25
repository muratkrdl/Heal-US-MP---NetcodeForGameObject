using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class PotionSpawnPoint : NetworkBehaviour
{
    [SerializeField] Vector2 spawnPotionsRange;

    [SerializeField] GameObject[] potions;

    float randomSpawnRate;
    int randomChoosenPotion;

    GameObject spawnPotion;

    GameObject spawnedPotion;

    void Start() 
    {
        if(!IsHost) return;
        StartTimerForPotion();
    }

    public void StartTimerForPotion()
    {
        randomSpawnRate = Random.Range(spawnPotionsRange.x,spawnPotionsRange.y);
        randomChoosenPotion = Random.Range(0,4);
        StartCoroutine(SpawnPotions(randomSpawnRate,randomChoosenPotion));
    }

    IEnumerator SpawnPotions(float spawnRate,int choosenPotionIndex)
    {
        spawnPotion = choosenPotionIndex switch
        {
            1 => potions[0],
            2 => potions[1],
            3 => potions[2],
            _ => potions[3]
        };

        yield return new WaitForSeconds(spawnRate);
        
        spawnedPotion = Instantiate(spawnPotion,transform.position,Quaternion.identity,transform);
        spawnedPotion.GetComponent<IPickUp>().SetParentTransformAsValue(transform);
        spawnedPotion.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);

        StopCoroutine(SpawnPotions(spawnRate,choosenPotionIndex));
    }

    public void PotionPickedUp()
    {
        PotionPickedUpServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void PotionPickedUpServerRpc()
    {
        if(!IsHost) return;

        spawnedPotion.GetComponent<NetworkObject>().Despawn();
        StartTimerForPotion();
    }

}
