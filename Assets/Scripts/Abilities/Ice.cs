using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Ice : MonoBehaviour
{
    [SerializeField] int lifeTime;
    [SerializeField] float slowTime;
    [SerializeField] float slowAmount;

    [SerializeField] ParticleSystem iceFX;
    [SerializeField] BoxCollider boxCollider;

    public int SetSlowAmount
    {
        set
        {
            slowAmount = value;
        }
    }

    float slowAmountPercent;

    void Start()
    {
        Invoke(nameof(KYS),lifeTime);
        SetPercentSlowAmount();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent<FirstPersonController>(out var player))
        {
            player.MoveSpeed = player.GetInitialMoveSpeed / slowAmountPercent;
            player.SprintSpeed = player.GetInitialSprintSpeed / slowAmountPercent;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<FirstPersonController>(out var player))
        {
            player.MoveSpeed = player.GetInitialMoveSpeed;
            player.SprintSpeed = player.GetInitialSprintSpeed;
        }
    }

    void KYS()
    {
        iceFX.Stop();
        boxCollider.size = Vector3.zero;
        Destroy(gameObject,slowTime + 5);
    }

    public void SetPercentSlowAmount()
    {
        slowAmountPercent = slowAmount / 25;
    }

}
