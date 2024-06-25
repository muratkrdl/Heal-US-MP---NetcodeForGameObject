using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityHeal : NetworkBehaviour
{
    [SerializeField] Image healFillArea;

    [SerializeField] FPSAnimation fpsAnimator;
    [SerializeField] CharacterAnimation characterAnimation;

    [SerializeField] Transform playerCam;
    [SerializeField] LayerMask layerMask;
    [SerializeField] KeyCode keyCode;
    [SerializeField] float range;

    bool isHealing;

    void Start() 
    {
        if(!IsOwner) enabled = false;

        HideVisualArea();
    }

    void Update() 
    {
        if(Physics.Raycast(playerCam.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, range, layerMask))
        {
            var villager = hit.transform.GetComponent<Villager>();
            if(!villager.GetIsInfected || villager.GetIsDead) 
            {
                StopHealing();
                StopAllCoroutines();
                return;
            }
            if(Input.GetKeyDown(keyCode))
            {
                UsingAbilityResetAnim();
                fpsAnimator.Heal();
                characterAnimation.Heal();
            }
            else if(Input.GetKeyUp(keyCode) || villager.GetIsDead)
            {
                StopHealing();
            }
        }
        else
        {
            if(!isHealing) { return; }
            StopHealing();
        }
    }

    void StopHealing()
    {
        HealingFalse();
        fpsAnimator.Idle();
        fpsAnimator.SetTrueCanUseAbility();
        HideVisualArea();
        StopAllCoroutines();
    }

    void UsingAbilityResetAnim()
	{
		fpsAnimator.SetFalseCanUseAbility();
		ResetWalkAnimation();
	}

    public void ResetWalkAnimation()
	{
		fpsAnimator.SetWalkAnimationFalse();
	}

    public void HealAnimationEvent()
    {
        if(Physics.Raycast(playerCam.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, range,layerMask))
        {
            if(hit.transform.TryGetComponent<Villager>(out var villager))
            {
                if(villager.GetIsInfected)
                {
                    villager.GethHealDamageServerRpc(false, 15);
                    HideVisualArea();
                }
            }
        }
    }

    public void StartFillArea()
    {
        HideVisualArea();
        if(IsOwner)
        {
            StartCoroutine(StartRoutine());
        }
    }

    public void HideVisualArea()
    {
        healFillArea.fillAmount = 0;
    }

    IEnumerator StartRoutine()
    {
        float elapsed = 0;
        float timer = 1.3f;
        float t = 0;
        while(true)
        {
            if(Physics.Raycast(playerCam.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, range,layerMask))
            {
                if(hit.transform.GetComponent<Villager>().GetIsDead)
                {
                    HideVisualArea();
                    break;
                }
            }
            elapsed += Time.deltaTime;
            t = elapsed / timer;
            healFillArea.fillAmount = Mathf.Lerp(healFillArea.fillAmount, 1, Time.deltaTime * t);
            if(healFillArea.fillAmount + .016f >= 1)
            {
                healFillArea.fillAmount = 1;
                StopCoroutine(StartRoutine());
                break;
            }
            yield return null;
        }
        StopCoroutine(StartRoutine());
    }

    public void HealingTrue()
    {
        isHealing = true;
    }
    public void HealingFalse()
    {
        isHealing = false;
    }
}
