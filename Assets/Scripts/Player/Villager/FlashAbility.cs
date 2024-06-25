using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class FlashAbility : NetworkBehaviour
{
    [SerializeField] GameObject[] flashObjPrefabs;
    
    [SerializeField] ParticleSystem daffodilVFX;
    [SerializeField] ParticleSystem hycanithVFX;
    [SerializeField] ParticleSystem yellowMushroomVFX;
    [SerializeField] ParticleSystem purpleMushroomVFX;
    [SerializeField] ParticleSystem potionVFX;

    [SerializeField] Transform flashOutPos;

    [SerializeField] Inventory inventory;

    [SerializeField] KeyCode keyCode;

    [SerializeField] FPSAnimation fPSAnimation;
    [SerializeField] CharacterAnimation characterAnimation;

    [SerializeField] FirstPersonController firstPersonController;

    bool usingFlash;

    public bool UsingFlash
    {
        get
        {
            return usingFlash;
        }
        set
        {
            usingFlash = value;
        }
    }

    void Start() 
    {
        if(!IsOwner) enabled = false;
    }

    void Update()
    {
        if(firstPersonController.GetEscMenu.GetIsThinking) return;

        if(Input.GetKeyDown(keyCode) && fPSAnimation.GetCanUseAbility)
        {
            if(!inventory.CanUseFlash()) { return;}
            
            fPSAnimation.Flash();
            characterAnimation.Flash();
        }
    }

    public void FlashAnimationEvent()
    {
        if(!inventory.CanUseFlash()) { return; }

        int prefabIndex = inventory.GetItemType() switch
        {
            PlantType.daffodil => 0,
            PlantType.hyacinth => 1,
            PlantType.yellowmushroom => 2,
            PlantType.purplemushroom => 3,
            _ => 4
        };
        
        inventory.DecreaseItemAmount();
        
        FlashVFX(prefabIndex);
        
        SoundManager.Instance.PlaySound3D("Throw Plant", transform.position); // herkese çal
        
        SpawnFlashObjServerRpc(prefabIndex);

        usingFlash = false;
    }

    [ServerRpc(RequireOwnership = false)] void SpawnFlashObjServerRpc(int index)
    {
        GameObject spawnedObj = Instantiate(flashObjPrefabs[index], flashOutPos);
        spawnedObj.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
    }

    void FlashVFX(int index)
    {
        FlashVFXServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)] void FlashVFXServerRpc(int index)
    {
        FlashVFXClientRpc(index);
    }

    [ClientRpc] void FlashVFXClientRpc(int index)
    {
        ParticleSystem playVFX = index switch
        {
            0 => daffodilVFX,
            1 => hycanithVFX,
            2 => yellowMushroomVFX,
            3 => purpleMushroomVFX,
            _ => potionVFX
        };

        playVFX.Play();
    }

}
