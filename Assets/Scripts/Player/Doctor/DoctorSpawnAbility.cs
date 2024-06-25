using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoctorSpawnAbility : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI spawnCDText;
    [SerializeField] Image spawnBG;

    [SerializeField] GameObject stoneMonsterPrefab;
    [SerializeField] Camera cam;

    [SerializeField] int abilityCD;

    [SerializeField] float manaCost;

    [SerializeField] int minSpawnMonsterCount;
    [SerializeField] int maxSpawnMonsterCount;

    [SerializeField] float minLifeTime;
    [SerializeField] float maxLifeTime;

    Transform patrick;

    int currentAbilityCD = 0;

    bool canUseSpawn = false;

    public float GetManaCost
    {
        get
        {
            return manaCost;
        }
    }

    public bool GetCanUseSpawn
    {
        get
        {
            return canUseSpawn;
        }
    }

    void Start() 
    {
        DisableSpawnCDCounter();
        Invoke(nameof(SetPatrickTarget), 1f);
    }

    void SetPatrickTarget()
    {
        patrick = GameObject.FindGameObjectWithTag("Patrick").transform;
        canUseSpawn = true;
    }

    public void StartSpawnStoneMonsters()
    {
        Mana.Instance.DecreaseMana(manaCost);

        SpawnStoneMonsters();
    }

    void SpawnStoneMonsters()
    {
        SpawnStoneMonstersServerRpc();
        
        StartCoroutine(UseSpawnAbilityCo()); // cd
    }

    [ServerRpc(RequireOwnership = false)] void SpawnStoneMonstersServerRpc()
    {
        var monsterCount = Random.Range(minSpawnMonsterCount,maxSpawnMonsterCount);
        var spawnPos = transform.position + transform.forward * 2;
        for (int i = 0; i < monsterCount; i++)
        {
            if(i %2 == 1) { spawnPos = transform.position + -transform.right * 1.5f; }
            else { spawnPos = transform.position + transform.right * 1.5f; }
            var monster = Instantiate(stoneMonsterPrefab, spawnPos, Quaternion.identity);
            monster.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
            monster.GetComponent<StoneMonster>().SetSpawnValues(patrick, Random.Range(minLifeTime,maxLifeTime), spawnPos);
            SoundManager.Instance.PlaySound3D("Spawn Ability", monster.transform.position); // herkese çal
        }
    }

    IEnumerator UseSpawnAbilityCo()
    {
        canUseSpawn = false;

        currentAbilityCD = abilityCD;
        EnableSpawnCDCounter();
        SetSpawnCDText();
      
        for (int i = 0; i < abilityCD; i++)
        {   
            yield return new WaitForSeconds(1);
            currentAbilityCD--;
            SetSpawnCDText();
        }
       
        canUseSpawn = true;
        StopCoroutine(UseSpawnAbilityCo());
    }

    void SetSpawnCDText()
    {
        spawnCDText.text = currentAbilityCD.ToString();
        if(currentAbilityCD == 0)
        {
            DisableSpawnCDCounter();
        }
    }

    void EnableSpawnCDCounter()
    {
        spawnCDText.enabled = true;
        spawnBG.enabled = true;
    }

    void DisableSpawnCDCounter()
    {
        spawnCDText.enabled = false;
        spawnBG.enabled = false;
    }

}
