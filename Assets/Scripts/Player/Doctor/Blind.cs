using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class Blind : NetworkBehaviour
{
    [SerializeField] float blindTime;

    [SerializeField] Image[] blindStars;

    [SerializeField] Color daffodilColor;
    [SerializeField] Color hycanithColor;
    [SerializeField] Color yellowMushroomColor;
    [SerializeField] Color purpleMushroomColor;
    [SerializeField] Color potionColor;

    [SerializeField] Animator starsAnimator;

    [SerializeField] Image blindImage;

    void Start() 
    {
        if(!IsOwner) enabled = false;
    }

    void OnTriggerEnter(Collider other) 
    {
        if(!IsOwner) return;

        if(other.transform.CompareTag("Flash"))
        {
            PlantType type = other.transform.GetComponent<FlashObj>().GetPlantType;

            if(type == PlantType.potion)
            {
                GetComponent<DoctorCollisionParticle>().StartGetDamageFromPoison(Random.Range(400, 601));
            }

            Color myColor = type switch
            {
                PlantType.daffodil => daffodilColor,
                PlantType.hyacinth => hycanithColor,
                PlantType.yellowmushroom => yellowMushroomColor,
                PlantType.purplemushroom => purpleMushroomColor,
                _ => potionColor,
            };

            if(myColor == null)
            {
                return;
            }
            
            SetStarsColor(myColor);

            float realBlindTime =  type switch
            {
                PlantType.daffodil => blindTime,
                PlantType.hyacinth => blindTime,
                PlantType.yellowmushroom => blindTime,
                PlantType.purplemushroom => blindTime,
                _ => blindTime / 4,
            };

            other.GetComponentInChildren<FlashObj>().KYS();
            SetFadeBlindStars();
            Invoke(nameof(SetUnFadeBlindStars), realBlindTime);
        }
    }

    void SetUnFadeBlindStars()
    {
        SetBlindImageStateServerRpc(false);
        starsAnimator.SetTrigger("UnFade");
    }

    void SetFadeBlindStars()
    {
        SetBlindImageStateServerRpc(true);
        starsAnimator.SetTrigger("Fade");
    }

    [ServerRpc(RequireOwnership = false)] void SetBlindImageStateServerRpc(bool _state)
    {
        SetBlindImageStateClientRpc(_state);
    }

    [ClientRpc] void SetBlindImageStateClientRpc(bool _state)
    {
        blindImage.gameObject.SetActive(_state);
    }

    void SetStarsColor(Color myColor)
    {
        foreach (var item in blindStars)
        {
            item.color = myColor;
        }
    }

}
