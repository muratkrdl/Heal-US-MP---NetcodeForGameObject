using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DoctorSpawnAbility : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI spawnCDText;
    [SerializeField] Image spawnBG;

    [SerializeField] PhotonView PV;

    [SerializeField] GameObject stoneMonsterPrefab;
    [SerializeField] Camera cam;

    [SerializeField] int abilityCD;

    [SerializeField] float damage;
    [SerializeField] float manaCost;

    [SerializeField] int minSpawnMonsterCount;
    [SerializeField] int maxSpawnMonsterCount;

    [SerializeField] float minLifeTime;
    [SerializeField] float maxLifeTime;

    Transform patrick;

    int currentAbilityCD = 0;

    bool canUseSpawn = false;

    int monsterCount;

    Vector3 spawnPos;

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
        PV.RPC(nameof(SpawnStoneMonsters),RpcTarget.All);
    }

    [PunRPC] void SpawnStoneMonsters()
    {
        monsterCount = Random.Range(minSpawnMonsterCount,maxSpawnMonsterCount);
        spawnPos = transform.position + transform.forward * 2;
        for (int i = 0; i < monsterCount; i++)
        {
            var monster = Instantiate(stoneMonsterPrefab,transform.position,Quaternion.identity);
            monster.GetComponent<StoneMonster>().SetTarget = patrick;
            monster.GetComponent<StoneMonster>().SetDamage = damage;
            monster.GetComponent<StoneMonster>().SetLifeTime = Random.Range(minLifeTime,maxLifeTime);
            monster.transform.position = spawnPos;
            SoundManager.Instance.RPCPlaySound3D("Spawn Ability", monster.transform.position);
            if(i %2 == 1) { spawnPos = transform.position + -transform.right * 1.5f;  }
            else { spawnPos = transform.position + transform.right * 1.5f;  }
        }
        if(PV.IsMine)
        {
            StartCoroutine(UseSpawnAbilityCo());
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
