using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ManaPickup : NetworkBehaviour, IPickUp
{
    [SerializeField] float stopParticleTime;

    [SerializeField] ParticleSystem spawnParticleSystem;

    Transform parent;

    void Start() 
    {
        Invoke(nameof(StopParticleSystem),stopParticleTime);
        if(!IsOwner) return;
        transform.SetParent(parent);
    }

    void StopParticleSystem()
    {
        spawnParticleSystem.Stop();
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if(other.GetComponent<CharacterController>() != null && !other.CompareTag("Villager"))
        {
            ManaPotions playerPotionData = other.GetComponentInChildren<ManaPotions>();
            if(playerPotionData.GetCurrentCount >= playerPotionData.GetMaxCount) { return; }
            SoundManager.Instance.PlaySound3D("Pick Up",transform.position);
            playerPotionData.IncreasePotionCount();
            
            GetComponentInParent<PotionSpawnPoint>().PotionPickedUp();
        }
    }

    public void SetParentTransformAsValue(Transform transform)
    {
        parent = transform;
    }
    
}
