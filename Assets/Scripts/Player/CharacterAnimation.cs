using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] float speedUpAnimatonSpeed;
    [SerializeField] float normalAnimatonSpeed;

    [SerializeField] FirstPersonController firstPersonController;

    bool canUseAbility = true;

    int index;

    public int SetRandomIndexForDoctor
    {
        set
        {
            index = value;
        }
    }

    public bool GetCanUseAbility
    {
        get
        {
            return canUseAbility;
        }
    }

    public void SetCharacterAnimationFloat(Vector2 input)
    {
        animator.SetFloat("x",input.x,.1f,Time.deltaTime);
		animator.SetFloat("y",input.y,.1f,Time.deltaTime);
    }

    public void SetTrueCanUseAbility()
    {
        if(!canUseAbility)
        {
            canUseAbility = true;
        }
    }
    public void SetFalseCanUseAbility()
    {
        if(canUseAbility)
        {
            canUseAbility = false;
        }
    }

    public void Fireball()
    {
        animator.SetTrigger("Fireball");
    }

    public void Lightning()
    {
        animator.SetTrigger("Lightning");
    }

    public void Poison()
    {
        animator.SetTrigger("Poison");
    }

    public void ThrowIcePotion()
    {
        animator.SetTrigger("Throw");
    }

    public void AbilityFinEvent()
    {
        SetTrueCanUseAbility();
    }

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Heal()
    {
        animator.SetTrigger("Heal");
    }

    public void GetDamage()
    {
        animator.SetTrigger("Get Damage");
    }

#region Doctor
    public void Melee()
    {
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
    public void HighExplosion()
    {
        animator.SetTrigger("High Explosion");
    }
    public void StoneMonster()
    {
        animator.SetTrigger("Stone Monster");
    }
#endregion

    public void SetPlayerSpeedToHalf()
    {
        if(firstPersonController.MoveSpeed != firstPersonController.GetInitialMoveSpeed / 2)
        {
            firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed / 2;
        }
        if(firstPersonController.SprintSpeed != firstPersonController.GetInitialSprintSpeed / 2)
        {
            firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed / 2;
        }
    }

    public void SetPlayerSpeedToNormal()
    {
        if(firstPersonController.MoveSpeed != firstPersonController.GetInitialMoveSpeed)
        {
            firstPersonController.MoveSpeed = firstPersonController.GetInitialMoveSpeed;
        }
        if(firstPersonController.SprintSpeed != firstPersonController.GetInitialSprintSpeed)
        {
            firstPersonController.SprintSpeed = firstPersonController.GetInitialSprintSpeed;
        }
    }

    public void SpeedUpAnimatorSpeed()
    {
        if(animator.speed == speedUpAnimatonSpeed) { return; }
        animator.speed = speedUpAnimatonSpeed;
    }

    public void NormalAnimatorSpeed()
    {
        if(animator.speed == normalAnimatonSpeed) { return; }
        animator.speed = normalAnimatonSpeed;
    }

    public void Flash()
    {
        SetFalseCanUseAbility();
        animator.SetTrigger("Flash");
    }

}
