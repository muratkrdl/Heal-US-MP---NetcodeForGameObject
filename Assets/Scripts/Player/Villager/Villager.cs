using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Villager : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] float decreaseHPTime;
    [SerializeField] float increaseHPTime;

    [SerializeField] float increaseSpeedAmount;
    [SerializeField] float increaseSpeedTime;

    [SerializeField] CharacterAnimation characterAnimatorScript;
	[SerializeField] FPSAnimation fpsAnimatorScript;

    [SerializeField] ParticleSystem healVFX;
    [SerializeField] ParticleSystem infectedVFX;

    [SerializeField] PlayerHP playerHP;
    [SerializeField] Stamina playerStamina;

    [SerializeField] FirstPersonController firstPersonController;
    PlayerManager playerManager;

    bool isInfected;

    public bool GetIsInfected
    {
        get
        {
            return isInfected;
        }
    }

    public bool GetIsDead
    {
        get
        {
            return playerHP.GetIsDead;
        }
    }

    void Start() 
    {
        if(!PV.IsMine) { return; }
        playerManager = firstPersonController.GetPlayerManager;
    }

    public void GetHeal()
    {
        PV.RPC(nameof(RPC_GetHealFromPlayer),RpcTarget.All, (float)15);
    }

    public void GetInfecte(float damage)
    {
        PV.RPC(nameof(RPC_GetDamageFromMonster),RpcTarget.All,damage);
    }

    [PunRPC] void RPC_GetHealFromPlayer(float damage)
    {
        if(!isInfected) { return; }

        firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed * increaseSpeedAmount;
        firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed * increaseSpeedAmount;
        Invoke(nameof(SetNormalSpeed), increaseSpeedTime);

        playerHP.IncreaseHP(damage);
        if(PV.IsMine)
        {
            StopAllCoroutines();
            StartCoroutine(IncreaseHPWithTime());
        }
        SoundManager.Instance.PlaySound3D("Heal",transform.position);
        infectedVFX.Stop();
        healVFX.Play();
        isInfected = false;
    }

    [PunRPC] void RPC_GetDamageFromMonster(float damage)
    {
        if(isInfected) { return; }

        playerHP.DecreaseHP(damage);
        isInfected = true;

        if(PV.IsMine)
        {
            StopAllCoroutines();
            StartCoroutine(DecreaseHPWithTime());
        }

        SoundManager.Instance.PlaySound3D("Infect",transform.position);
        infectedVFX.Play();
        if(!PV.IsMine) { return; }
        characterAnimatorScript.GetDamage();
        fpsAnimatorScript.GetDamage();
    }

    void SetNormalSpeed()
    {
        firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed;
        firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed;
    }

    public void CloseVFX()
    {
        PV.RPC(nameof(RPC_CloseVFX),RpcTarget.All);
    }

    [PunRPC] void RPC_CloseVFX()
    {
        infectedVFX.Stop();
    }

    IEnumerator DecreaseHPWithTime()
    {
        while(isInfected)
        {
            yield return new WaitForSeconds(decreaseHPTime);
            playerHP.DecreaseHP(1);
        }
        StopAllCoroutines();
    }

    IEnumerator IncreaseHPWithTime()
    {
        while(!isInfected || playerHP.GetCurrentHP < playerHP.MaxHP)
        {
            yield return new WaitForSeconds(increaseHPTime);
            playerHP.IncreaseHP(1);
        }
        StopAllCoroutines();
    }

}
