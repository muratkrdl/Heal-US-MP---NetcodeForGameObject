using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;

    [SerializeField] float speedUpAnimatonSpeed;
    [SerializeField] float normalAnimatonSpeed;

    [SerializeField] FirstPersonController firstPersonController;

    int index;

    public int SetRandomIndexForDoctor
    {
        set
        {
            index = value;
        }
    }

    public void SetCharacterAnimationFloat(Vector2 input)
    {
        animator.SetFloat("x",input.x,.1f,Time.deltaTime);
		animator.SetFloat("y",input.y,.1f,Time.deltaTime);
    }

    public void SetCharacterAnimationFloatDirectly(Vector2 input)
    {
        animator.SetFloat("x",input.x);
		animator.SetFloat("y",input.y);
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

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Heal()
    {
        animator.SetTrigger("Heal");
    }

    public void Die()
    {
        animator.SetTrigger("Die");
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
        animator.SetTrigger("Flash");
    }

    public void SetSecondLayerWeight(float amount)
    {
        animator.SetLayerWeight(1, amount);
    }

}
