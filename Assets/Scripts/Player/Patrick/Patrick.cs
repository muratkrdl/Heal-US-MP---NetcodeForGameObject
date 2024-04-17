using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Patrick : MonoBehaviour
{
	[SerializeField] PhotonView PV;

    [Header("KeyCodes")]
	[SerializeField] KeyCode fireballKeycode;
	[SerializeField] KeyCode lightningKeycode;
	[SerializeField] KeyCode poisonKeycode;
	[SerializeField] KeyCode useHPPotionKeycode;
	[SerializeField] KeyCode useManaPotionKeycode;
	[SerializeField] KeyCode useStaminaPotionKeycode;
	[SerializeField] KeyCode useIcePotionKeycode;
    
	[Space(10)]
	[Header("Abilities & Others")]
    //[SerializeField] Transform skills;
	[SerializeField] AbilityFireball abilityFireball;
	[SerializeField] AbilityLightning abilityLightning;
	[SerializeField] AbilityPoison abilityPoison;

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

    PlayerManager playerManager;

    void Start() 
    {
		if(!PV.IsMine) { return; }
        playerManager = firstPersonController.GetPlayerManager;
    }

    void Update() 
    {
		if(!PV.IsMine) { return; }
        UseAbility();
        UsePotion();
    }

    void UseAbility()
	{
		if(Input.GetKeyDown(fireballKeycode) && characterAnimatorScript.GetCanUseAbility && abilityFireball.GetCanThrowFireball && Mana.Instance.GetCurrentMana >= abilityFireball.GetManaCost)
		{
			UseFireball();
		}
		else if(Input.GetKeyDown(lightningKeycode) && characterAnimatorScript.GetCanUseAbility && abilityLightning.GetCanUseLightning && Mana.Instance.GetCurrentMana >= abilityLightning.GetManaCost)
		{
			UseLightning();
		}
		else if(Input.GetKeyDown(poisonKeycode) && characterAnimatorScript.GetCanUseAbility && abilityPoison.GetCanUsePoison && Mana.Instance.GetCurrentMana >= abilityPoison.GetManaCost)
		{
			UsePoison();
		}
		else if(Input.GetKeyDown(useIcePotionKeycode) && characterAnimatorScript.GetCanUseAbility && icePotions.GetCurrentCount > 0)
		{
			UseIcePotion();
		}
	}

	void UseFireball()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.Fireball();
		fpsAnimatorScript.Fireball();
	}

	void UseLightning()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.Lightning();
		fpsAnimatorScript.Lightning();
	}

	void UsePoison()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.Poison();
		fpsAnimatorScript.Poison();
	}

	void UseIcePotion()
	{
		UsingAbilityResetAnim();
		characterAnimatorScript.ThrowIcePotion();
		fpsAnimatorScript.ThrowIcePotion();
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
		characterAnimatorScript.SetFalseCanUseAbility();
		characterAnimatorScript.SetPlayerSpeedToHalf();
        firstPersonController.ResetWalkAnimation();
	}

	public void GetDamage(float damage)
	{
		PV.RPC(nameof(RPC_GetDamage), RpcTarget.All, damage);
	}

	[PunRPC] void RPC_GetDamage(float damage)
	{
		playerHP.DecreaseHP(damage);
	}

}
