using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Patrick : NetworkBehaviour
{
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

	void Start() 
	{
		if(!IsOwner) enabled = false;
	}

    void Update() 
    {
        UseAbility();
        UsePotion();
    }

    void UseAbility()
	{
		if(Input.GetKeyDown(fireballKeycode) && fpsAnimatorScript.GetCanUseAbility && abilityFireball.GetCanThrowFireball && Mana.Instance.GetCurrentMana >= abilityFireball.GetManaCost)
		{
			UseFireball();
		}
		else if(Input.GetKeyDown(lightningKeycode) && fpsAnimatorScript.GetCanUseAbility && abilityLightning.GetCanUseLightning && Mana.Instance.GetCurrentMana >= abilityLightning.GetManaCost)
		{
			UseLightning();
		}
		else if(Input.GetKeyDown(poisonKeycode) && fpsAnimatorScript.GetCanUseAbility && abilityPoison.GetCanUsePoison && Mana.Instance.GetCurrentMana >= abilityPoison.GetManaCost)
		{
			UsePoison();
		}
		else if(Input.GetKeyDown(useIcePotionKeycode) && fpsAnimatorScript.GetCanUseAbility && icePotions.GetCurrentCount > 0)
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
		if(firstPersonController.GetEscMenu.GetIsThinking) return;

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

	public void GetDamage(float damage)
	{
		GetDamageServerRpc(damage);
	}

	[ServerRpc(RequireOwnership = false)] void GetDamageServerRpc(float damage)
	{
		GetDamageClientRpc(damage);
	}	

	[ClientRpc] void GetDamageClientRpc(float damage)
	{
		playerHP.DecreaseHP(damage);
	}

}
