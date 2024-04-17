using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HPPickup : MonoBehaviour, IPickUp
{
    [SerializeField] PhotonView PV;
    [SerializeField] float stopParticleTime;

    [SerializeField] ParticleSystem spawnParticleSystem;

    Transform parent;

    void Start() 
    {
        transform.SetParent(parent);
        Invoke(nameof(StopParticleSystem),stopParticleTime);
    }

    void StopParticleSystem()
    {
        spawnParticleSystem.Stop();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.GetComponent<CharacterController>() != null && !other.CompareTag("Villager"))
        {
            HPPotions playerPotionData = other.GetComponentInChildren<HPPotions>();
            if(playerPotionData.GetCurrentCount >= playerPotionData.GetMaxCount) { return; }
            SoundManager.Instance.PlaySound3D("Pick Up",transform.position);
            playerPotionData.IncreasePotionCount();
            if(GetComponentInParent<PotionSpawnPoint>() != null)
            {
                parent.GetComponent<PotionSpawnPoint>().StartTimerForPotion();
            }
            PV.RPC(nameof(KYS),RpcTarget.All);
        }
    }

    [PunRPC] void KYS()
    {
        Destroy(gameObject);
    }

    public void SetParentTransformAsValue(Transform transform)
    {
        parent = transform;
    }

}
