using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class Blind : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] float blindTime;

    [SerializeField] Image[] blindStars;

    [SerializeField] Color daffodilColor;
    [SerializeField] Color hycanithColor;
    [SerializeField] Color yellowMushroomColor;
    [SerializeField] Color purpleMushroomColor;
    [SerializeField] Color potionColor;

    [SerializeField] Animator starsAnimator;

    [SerializeField] Image blindImage;

    void OnTriggerEnter(Collider other) 
    {
        if(other.transform.CompareTag("Flash"))
        {
            if(other.transform.GetComponent<FlashObj>().GetPlantType == PlantType.potion)
            {
                GetComponent<DoctorCollisionParticle>().StartGetDamageFromPoison(Random.Range(400, 601));
            }

            Color myColor = other.transform.GetComponent<FlashObj>().GetPlantType switch
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

            float realBlindTime =  other.transform.GetComponent<FlashObj>().GetPlantType switch
            {
                PlantType.daffodil => blindTime,
                PlantType.hyacinth => blindTime,
                PlantType.yellowmushroom => blindTime,
                PlantType.purplemushroom => blindTime,
                _ => blindTime / 4,
            };

            SetStarsColor(myColor);
            SetFadeBlindStars();
            Invoke(nameof(SetUnFadeBlindStars), realBlindTime);
        }
    }

    void SetUnFadeBlindStars()
    {
        PV.RPC(nameof(RPC_SetBlindImage), RpcTarget.All, false);
        starsAnimator.SetTrigger("UnFade");
    }

    void SetFadeBlindStars()
    {
        PV.RPC(nameof(RPC_SetBlindImage), RpcTarget.All, true);
        starsAnimator.SetTrigger("Fade");
    }

    void SetStarsColor(Color myColor)
    {
        foreach (var item in blindStars)
        {
            item.color = myColor;
        }
    }

    [PunRPC] void RPC_SetBlindImage(bool value)
    {
        blindImage.gameObject.SetActive(value);
    }

}
