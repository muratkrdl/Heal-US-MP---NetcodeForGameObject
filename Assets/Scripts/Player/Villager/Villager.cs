using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Villager : NetworkBehaviour
{
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

    bool isInfected = false;

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

    [ServerRpc(RequireOwnership = false)] public void GethHealDamageServerRpc(bool value, float amount)
    {
        GethHealDamageClientRpc(value, amount);
    }

    [ClientRpc] void GethHealDamageClientRpc(bool value, float amount)
    {
        ChangeInfecteState(value);
        if(!IsOwner) return;

        if(value)
        {
            GetInfecte(amount);
        }
        else if(!value)
        {
            GetHeal();
        }
    }

    void GetHeal()
    {
        firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed * increaseSpeedAmount;
        firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed * increaseSpeedAmount;
        Invoke(nameof(SetNormalSpeed), increaseSpeedTime);

        playerHP.IncreaseHP(15);

        StopAllCoroutines();
        StartCoroutine(IncreaseHPWithTime());

        SoundManager.Instance.PlaySound3D("Heal",transform.position);
    }

    void GetInfecte(float damage)
    {
        playerHP.DecreaseHP(damage);

        StopAllCoroutines();
        StartCoroutine(DecreaseHPWithTime());

        SoundManager.Instance.PlaySound3D("Infect",transform.position);
        
        characterAnimatorScript.GetDamage();
        fpsAnimatorScript.GetDamage();
    }

    void ChangeInfecteState(bool value)
    {
        if(value)
        {
            infectedVFX.Play();
        }
        else if(!value)
        {
            infectedVFX.Stop();
            healVFX.Play();
        }
        
        isInfected = value;
    }

    void SetNormalSpeed()
    {
        firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed;
        firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed;
    }

    public void CloseVFX()
    {
        CloseVFXServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void CloseVFXServerRpc()
    {
        CloseVFXClientRpc();
    }

    [ClientRpc] void CloseVFXClientRpc()
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
