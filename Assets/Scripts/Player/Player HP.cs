using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Events;

public class PlayerHP : MonoBehaviour
{
    public static PlayerHP Instance;

    public UnityEvent villagerDead;

    [SerializeField] GameObject spawnFreeCamPrefab;

    [SerializeField] PhotonView PV;

    [SerializeField] Animator[] dieAnimator;

    [SerializeField] float maxHP;
    [SerializeField] float lerpTime;

    [SerializeField] Slider hpSlider;
    [SerializeField] TextMeshProUGUI valuesText;

    [SerializeField] FirstPersonController firstPersonController;

    PlayerManager playerManager;

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

    void Awake() 
    {
        if(!PV.IsMine) { return; }
        Instance = this;
    }

    void Start() 
    {
        SetStartValues();
        villagerDead.AddListener(Manager.Instance.CheckPatrickLose);
    }

    void SetStartValues()
    {
        hpSlider.maxValue = maxHP;
        currentHP = maxHP;
        targetHP = currentHP;
        hpSlider.value = currentHP;
        playerManager = GetComponentInParent<FirstPersonController>().GetPlayerManager;
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
                playerDead = true;
                SoundManager.Instance.PlaySound3D("Die", transform.position);
                
                if(!PV.IsMine) { break; }
                if(playerManager.GetHumanType == HumanType.patrick)
                {
                    Manager.Instance.PatrickLoseGame();
                }
                else if(playerManager.GetHumanType == HumanType.doctor)
                {
                    Manager.Instance.PatrickWinGame();
                }
                else
                {
                    Hashtable playerProps = new()
                    {
                        { "Dead", true }
                    };
                    PhotonNetwork.SetPlayerCustomProperties(playerProps);
                    villagerDead.Invoke();
                    GetComponentInParent<Villager>().CloseVFX();
                }

                DieAnimation();

                if(playerManager.GetHumanType == HumanType.villager)
                    Invoke(nameof(SpawnFreeCamera), 1.5f);
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
        PV.RPC(nameof(RPC_Animation), RpcTarget.All);
    }

    [PunRPC] void RPC_Animation()
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
