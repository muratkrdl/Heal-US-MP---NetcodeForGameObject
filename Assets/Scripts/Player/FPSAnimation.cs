using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] Animator threeDModelAnimator;

    [SerializeField] CharacterAnimation characterAnimation;

    [SerializeField] AbilityFireball abilityFireball;
    [SerializeField] AbilityLightning abilityLightning;
    [SerializeField] AbilityPoison abilityPoison;
    [SerializeField] UseIcePotion useIcePotion;
    [SerializeField] AbilityHeal abilityHeal;

    [SerializeField] DoctorExplosionAbility doctorExplosionAbility;
    [SerializeField] DoctorSpawnAbility doctorSpawnAbility;
    [SerializeField] DoctorMelee doctorMelee;

    [SerializeField] FlashAbility flashAbility;

    bool canUseAbility = true;

    public bool GetCanUseAbility
    {
        get
        {
            return canUseAbility;
        }
    }

    public void SetTrueCanUseAbility()
    {
        canUseAbility = true;
        threeDModelAnimator.SetBool("CanUseAbility", canUseAbility);
        characterAnimation.SetSecondLayerWeight(0);
    }
    public void SetFalseCanUseAbility()
    {
        canUseAbility = false;
        threeDModelAnimator.SetBool("CanUseAbility", canUseAbility);
        characterAnimation.SetSecondLayerWeight(1);
    }

    public void UseAbilityStart()
    {
        SetFalseCanUseAbility();
        if(flashAbility != null)
        {
            flashAbility.UsingFlash = true;
        }
    }

    public void AbilityFinEvent()
    {
        SetTrueCanUseAbility();
    }

#region Fireball
    public void Fireball()
    {
        animator.SetTrigger("fireball");
    }
    public void FireballAnimationEvent()
    {
        abilityFireball.ThrowFireBall();
    }
    public void FireBallSoundEvent()
    {
        SoundManager.Instance.PlaySound3D("Fireball",transform.position);
    }
#endregion

#region Lightning
    public void Lightning()
    {
        animator.SetTrigger("lightning");
    }
    public void LightningAnimationEvent()
    {
        abilityLightning.UseLightning();
    }
#endregion

#region Poison
    public void Poison()
    {
        animator.SetTrigger("poison");
    }
    public void PoisonAnimationEvent()
    {
        abilityPoison.OpenPoison();
    }
    public void PoisonAnimationFinEvent()
    {
        abilityPoison.ClosePoison();
    }
    public void PoisonSoundEvent()
    {
        SoundManager.Instance.PlaySound3D("Poison",transform.position);
    }
#endregion

#region Throw
    public void ThrowIcePotion()
    {
        animator.SetTrigger("throw");
    }
    public void IceThrowSoundEvent()
    {
        SoundManager.Instance.PlaySound3D("Throw",transform.position);
    }
    public void UseIcePotionAnimationEvent()
    {
        useIcePotion.UseIcePotionAsAbility();
    }
#endregion

#region Walk
    public bool GetWalkAnimationBool()
    {
        return animator.GetBool("walk");
    } 
    public void SetWalkAnimationTrue()
    {
        animator.SetBool("walk",true);
    }
    public void SetWalkAnimationFalse()
    {
        animator.SetBool("walk",false);
    }
#endregion

#region Heal
    public void Heal()
    {
        animator.SetTrigger("heal");
    }
    public void HealingTrue()
    {
        abilityHeal.HealingTrue();
    }
    public void HealStartVisualArea()
    {
        abilityHeal.StartFillArea();
    }
    public void HealAnimationEvent()
    {
        abilityHeal.HealAnimationEvent();
    }
#endregion

#region Doctor
    public void Melee()
    {
        int index = Random.Range(0,3);
        characterAnimation.SetRandomIndexForDoctor = index;
        if(index == 0)
        {
            animator.SetTrigger("Melee 1");
        }
        else if(index == 1)
        {
            animator.SetTrigger("Melee 2");
        }
        else
        {
            animator.SetTrigger("Melee 3");
        }
    }
    public void MeleeSFX()
    {
        SoundManager.Instance.PlaySound3D("Melee", transform.position);
    }
    public void MeleeAnimationEvent()
    {
        doctorMelee.MeleeAnimationEvent();
    }
    public void HighExplosion()
    {
        animator.SetTrigger("High Explosion");
    }
    public void HighExplosionAnimationEvent()
    {
        doctorExplosionAbility.UseHighExplosion();
    }
    public void StoneMonster()
    {
        animator.SetTrigger("Stone Monster");
    }
    public void StoneMonsterAnimationEvent()
    {
        doctorSpawnAbility.StartSpawnStoneMonsters();
    }
#endregion

    public void GetDamage()
    {
        animator.SetTrigger("Get Damage");
    }

    public void Idle()
    {
        if(GetWalkAnimationBool())
            SetWalkAnimationFalse();
        animator.SetTrigger("idle");
    }

    public void SetWalkMultiplier(float s)
    {
        if(animator.GetFloat("s") == s) { return; }
        animator.SetFloat("s", s);
    }

    public void Flash()
    {
        animator.SetTrigger("Flash");
    }

    public void FlashAnimationEvent()
    {
        flashAbility.FlashAnimationEvent();
    }

    public void PlaySFx(string str)
    {
        SoundManager.Instance.PlaySound3D(str, transform.position);
    }

}
