using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.Netcode;

public class PlayerHP : NetworkBehaviour
{
    public static PlayerHP Instance;

    public UnityEvent villagerDead;

    [SerializeField] GameObject spawnFreeCamPrefab;

    [SerializeField] Animator[] dieAnimator;

    [SerializeField] float maxHP;
    [SerializeField] float lerpTime;

    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI valuesText;

    [SerializeField] FirstPersonController firstPersonController;

    float currentHP;

    float targetHP;

    bool playerDead = false;

    public bool GetIsDead
    {
        get
        {
            return playerDead;
        }
    }

    public float MaxHP
    {
        get
        {
            return maxHP;
        }
        set
        {
            maxHP = value;
        }
    }

    public float GetCurrentHP
    {
        get
        {
            return currentHP;
        }
    }

    void Start() 
    {
        SetStartValues();
        
        villagerDead.AddListener(Manager.Instance.CheckPatrickLose);
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            Instance = this;
        }
    }

    void SetStartValues()
    {
        hpSlider.maxValue = maxHP;
        currentHP = maxHP;
        targetHP = currentHP;
        hpSlider.value = currentHP;
        UpdateCurrentValues();
    }

    public void IncreaseHP(float amount)
    {
        StartCoroutine(SmoothHPVisual(amount,true));
    }

    public void DecreaseHP(float amount)
    {
        StartCoroutine(SmoothHPVisual(-amount,false));
    }

#region decreaseserver
    public void DecreaseHPWithServer(float amount, ulong _id)
    {
        DecreaseHPServerRpc(amount, _id);
    }

    [ServerRpc(RequireOwnership = false)] void DecreaseHPServerRpc(float amount, ulong _id)
    {
        DecreaseHPClientRpc(amount, _id);
    }

    [ClientRpc] void DecreaseHPClientRpc(float amount, ulong _id)
    {
        if(_id == GetComponentInParent<NetworkObject>().OwnerClientId)
        {
            StartCoroutine(SmoothHPVisual(-amount,false));
        }
    }
#endregion
    
    IEnumerator SmoothHPVisual(float amount,bool increase)
    {
        targetHP = currentHP;
        targetHP += amount;
        while(!playerDead)
        {
            yield return null;
            if(amount > 0 && currentHP >= maxHP) 
            { 
                currentHP = maxHP;
                targetHP = currentHP;
                break;
            }

            currentHP = Mathf.Lerp(currentHP, targetHP, Time.deltaTime * lerpTime);
            hpSlider.value = currentHP;

            if(currentHP <= 0 + .2f)
            {
                //Die
                HumanType type = GetComponentInParent<FirstPersonController>().HumanType;
                playerDead = true;
                SoundManager.Instance.PlaySound3D("Die", transform.position);

                if(type == HumanType.patrick)
                {
                    Manager.Instance.PatrickLoseGame();
                }
                else if(type == HumanType.doctor)
                {
                    Manager.Instance.PatrickWinGame();
                }
                else if(type == HumanType.villager)
                {
                    villagerDead?.Invoke();
                    GetComponentInParent<Villager>().CloseVFX();
                }

                DieAnimation();

                if(type == HumanType.villager)
                {
                    Invoke(nameof(SpawnFreeCamera), 1.5f); // free cam
                }
            }

            if(Mathf.Abs(currentHP - targetHP) <= .02f)
            {
                if(increase) { currentHP += 1; }
                currentHP = targetHP;
                Mathf.RoundToInt(currentHP);
                hpSlider.value = currentHP;
                break;
            }
            UpdateCurrentValues();
        }
        
        UpdateCurrentValues();
        StopAllCoroutines();
    }

    void UpdateCurrentValues()
    {
        valuesText.text = ((int)hpSlider.value).ToString() + " / " + hpSlider.maxValue.ToString();
    }

    void DieAnimation()
    {
        foreach(Animator item in dieAnimator)
        {
            if(item == null) { continue; }
            if(!item.enabled)
                item.enabled = true;
            item.SetTrigger("Die");
        }
    }

    void SpawnFreeCamera()
    {
        firstPersonController.PlayerDead();
        Instantiate(spawnFreeCamPrefab, transform.position, Quaternion.identity);
    }

}
