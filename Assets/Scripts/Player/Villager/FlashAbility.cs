using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class FlashAbility : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] ParticleSystem daffodilVFX;
    [SerializeField] ParticleSystem hycanithVFX;
    [SerializeField] ParticleSystem yellowMushroomVFX;
    [SerializeField] ParticleSystem purpleMushroomVFX;
    [SerializeField] ParticleSystem potionVFX;

    [SerializeField] Transform flashOutPos;
    [SerializeField] Transform lookPos;

    [SerializeField] Inventory inventory;

    [SerializeField] KeyCode keyCode;

    [SerializeField] FPSAnimation fPSAnimation;
    [SerializeField] CharacterAnimation characterAnimation;

    const string DAFFODIL_PREFAB_NAME = "DaffodilFlashObj";
    const string HYCANITH_PREFAB_NAME = "HycanithFlashObj";
    const string YELLOW_MUSHROOM_PREFAB_NAME = "YellowMushroomFlashObj";
    const string PURPLE_MUSHROOM_PREFAB_NAME = "PurpleMushroomFlashObj";
    const string POISON_PREFAB_NAME = "PotionFlashObj";

    void Update()
    {
        if(!PV.IsMine) { return; }

        if(Input.GetKeyDown(keyCode) && characterAnimation.GetCanUseAbility)
        {
            if(!inventory.CanUseFlash()) { return;}
            
            fPSAnimation.Flash();
            characterAnimation.Flash();
        }
    }

    public void FlashAnimationEvemt()
    {
        if(!inventory.CanUseFlash()) { return; }

        string prefabName = inventory.GetItemType() switch
        {
            PlantType.daffodil => DAFFODIL_PREFAB_NAME,
            PlantType.hyacinth => HYCANITH_PREFAB_NAME,
            PlantType.yellowmushroom => YELLOW_MUSHROOM_PREFAB_NAME,
            PlantType.purplemushroom => PURPLE_MUSHROOM_PREFAB_NAME,
            _ => POISON_PREFAB_NAME
        };
        
        inventory.DecreaseItemAmount();
        PV.RPC(nameof(RPC_FlashVFX), RpcTarget.All, prefabName);
        
        SoundManager.Instance.RPCPlaySound3D("Throw Plant", transform.position);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", prefabName), flashOutPos.position, flashOutPos.rotation);
    }

    [PunRPC] void RPC_FlashVFX(string str)
    {
        ParticleSystem playVFX = str switch
        {
            DAFFODIL_PREFAB_NAME => daffodilVFX,
            HYCANITH_PREFAB_NAME => hycanithVFX,
            YELLOW_MUSHROOM_PREFAB_NAME => yellowMushroomVFX,
            PURPLE_MUSHROOM_PREFAB_NAME => purpleMushroomVFX,
            _ => potionVFX
        };
        playVFX.Play();
    }

}
