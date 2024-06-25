using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Doctor : NetworkBehaviour
{
    [SerializeField] ParticleSystem lightningFX;
    [SerializeField] ParticleSystem fireBallStunFX;

    [Header("KeyCodes")]
	[SerializeField] KeyCode stoneMonsterKeycode;
	[SerializeField] KeyCode highExplosionKeycode;
	[SerializeField] KeyCode meleeAttackKeycode;
	[SerializeField] KeyCode useHPPotionKeycode;
	[SerializeField] KeyCode useManaPotionKeycode;
	[SerializeField] KeyCode useStaminaPotionKeycode;
	[SerializeField] KeyCode useIcePotionKeycode;

    [SerializeField] DoctorSpawnAbility doctorSpawnAbility;
	[SerializeField] DoctorExplosionAbility doctorExplosionAbility;
	[SerializeField] DoctorMelee doctorMelee;

	[SerializeField] HPPotions hPPotions;
	[SerializeField] ManaPotions manaPotions;
	[SerializeField] StaminaPotions staminaPotions;
	[SerializeField] IcePotions icePotions;

	[SerializeField] UseHPPotion useHPPotion;
	[SerializeField] UseManaPotion useManaPotion;
	[SerializeField] UseStaminaPotion useStaminaPotion;

	[SerializeField] PlayerHP playerHP;
	[SerializeField] Mana playerMana;
	[SerializeField] Stamina playerStamina;

	[SerializeField] CharacterAnimation characterAnimatorScript;
	[SerializeField] FPSAnimation fpsAnimatorScript;

    [SerializeField] FirstPersonController firstPersonController;

    bool isStunned;

    public bool GetIsStunned
    {
        get
        {
            return isStunned;
        }
    }

	void Start() 
	{
		if(!IsOwner) enabled = false;	
	}

    void Update() 
    {
		if(firstPersonController.GetEscMenu.GetIsThinking) { return; }

        UseAbility();
        UsePotion();
    }

    void UseAbility()
	{
		if(Input.GetKeyDown(stoneMonsterKeycode) && fpsAnimatorScript.GetCanUseAbility && doctorSpawnAbility.GetCanUseSpawn && Mana.Instance.GetCurrentMana >= doctorSpawnAbility.GetManaCost)
		{
			UseStoneMonster();
		}
		else if(Input.GetKeyDown(highExplosionKeycode) && fpsAnimatorScript.GetCanUseAbility && doctorExplosionAbility.GetCanUseHighExplosion && Mana.Instance.GetCurrentMana >= doctorExplosionAbility.GetManaCost)
		{
			UseHighExplosion();
		}
		else if(Input.GetKeyDown(useIcePotionKeycode) && fpsAnimatorScript.GetCanUseAbility && icePotions.GetCurrentCount > 0)
		{
			UseIcePotion();
		}
		else if(Input.GetKeyDown(meleeAttackKeycode) && fpsAnimatorScript.GetCanUseAbility && doctorMelee.CanMeleeAttack)
		{
			MeleeAttack();
		}
	}

	void UseStoneMonster()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.StoneMonster();
		fpsAnimatorScript.StoneMonster();
	}

	void UseHighExplosion()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.HighExplosion();
		fpsAnimatorScript.HighExplosion();
	}

	void UseIcePotion()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.ThrowIcePotion();
		fpsAnimatorScript.ThrowIcePotion();
	}

	void MeleeAttack()
	{
		UsingAbilityResetAnim();
		fpsAnimatorScript.Melee();
        characterAnimatorScript.Melee();
		doctorMelee.CanMeleeAttack = false;
	}

	void UsePotion()
	{
		if(Input.GetKeyDown(useHPPotionKeycode) && hPPotions.GetCurrentCount > 0)
		{
			useHPPotion.UseHPPotionFunc();
		}
	 	else if(Input.GetKeyDown(useManaPotionKeycode) && manaPotions.GetCurrentCount > 0)
		{
			useManaPotion.UseManaPotionFunc();
		}
		else if(Input.GetKeyDown(useStaminaPotionKeycode) && staminaPotions.GetCurrentCount > 0)
		{
			useStaminaPotion.UseStaminaPotionFunc();
		}
	}

	void UsingAbilityResetAnim()
	{
		fpsAnimatorScript.SetFalseCanUseAbility();
        firstPersonController.ResetWalkAnimation();
	}

    public void MonsterStunned(float time,bool isLightning)
    {
		OpenVFX(isLightning);

        Invoke(nameof(FinishStunMonster),time);
    }

    void FinishStunMonster()
    {
		CloseVFXServerRpc();
    }

	[ServerRpc(RequireOwnership = false)] void CloseVFXServerRpc()
	{
		CloseVFXClientRpc();
	}

	[ClientRpc] void CloseVFXClientRpc()
	{
		isStunned = false;

		lightningFX.Stop();
		fireBallStunFX.Stop();
	}

	void OpenVFX(bool isLightning)
	{
        OpenVFXServerRpc(isLightning);
	}

	[ServerRpc(RequireOwnership = false)] void OpenVFXServerRpc(bool isLightning)
	{
		OpenVFXClientRpc(isLightning);
	}

	[ClientRpc] void OpenVFXClientRpc(bool isLightning)
	{
		isStunned = true;
		if(isLightning)
		{
			lightningFX.Play();
		}
		else if(!isLightning)
		{
			fireBallStunFX.Play();
		}
	}

}
