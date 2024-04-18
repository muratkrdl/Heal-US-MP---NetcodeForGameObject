using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlantType
{
    none,
    daffodil,
    hyacinth,
    yellowmushroom,
    purplemushroom,
    potion
}

public class Cauldron : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] Transform itemDropPos;

    [SerializeField] float cookTime;
    [SerializeField] float decreaseTime;
    [SerializeField] float updateSliderValueTime;

    [SerializeField] AudioSource boilingSFX;

    [SerializeField] GameObject liquid;
    [SerializeField] GameObject myLight;
    [SerializeField] ParticleSystem cookingVFX;

    [SerializeField] Transform taskCanvas;

    [SerializeField] GameObject cookingSliderBG;
    [SerializeField] Image cookingSlider;

    [SerializeField] GameObject readyItem;

    [SerializeField] Image daffodilTaskImage;
    [SerializeField] Image hyacinthTaskImage;
    [SerializeField] Image yellowMushroomTaskImage;
    [SerializeField] Image purpleMushroomTaskImage;

    [SerializeField] TextMeshProUGUI daffodilText;
    [SerializeField] TextMeshProUGUI hycainthText;
    [SerializeField] TextMeshProUGUI yellowMushroomText;
    [SerializeField] TextMeshProUGUI purpleMushroomText;

    [SerializeField] int maxDaffodil;
    [SerializeField] int maxHyacinth;
    [SerializeField] int maxYellowMushroom;
    [SerializeField] int maxPurpleMushroom;

    const string DAFFODIL_PREFAB_NAME = "Daffodil";
    const string HYCAINTH_PREFAB_NAME = "Hyacinth";
    const string YELLOW_MUSHROOM_PREFAB_NAME = "YellowMushroom";
    const string PURPLE_MUSHROOM_PREFAB_NAME = "PurpleMushroom";

    int currentDaffodil;
    int currentHyacinth;
    int currentYellowMushroom;
    int currentPurpleMushroom;

    float cookingSliderCurrentTime = 0;

    List<GameObject> plantObjs = new();

    bool itemReady = false;

    public bool GetItemReady
    {
        get
        {
            return itemReady;
        }
    }

    void Start() 
    {
        currentDaffodil = maxDaffodil;
        currentHyacinth = maxHyacinth;
        currentYellowMushroom = maxYellowMushroom;
        currentPurpleMushroom = maxPurpleMushroom;
        daffodilText.text = currentDaffodil.ToString();
        hycainthText.text = currentHyacinth.ToString();
        yellowMushroomText.text = currentYellowMushroom.ToString();
        purpleMushroomText.text = currentPurpleMushroom.ToString();
        cookingSlider.fillAmount = cookingSliderCurrentTime;
        cookingSliderBG.SetActive(false);
        liquid.SetActive(false);
        myLight.SetActive(false);
    }

    public void InteractWithCauldron(PlantType type)
    {
        string spawnObjName = type switch
        {
            PlantType.daffodil => DAFFODIL_PREFAB_NAME,
            PlantType.hyacinth => HYCAINTH_PREFAB_NAME,
            PlantType.yellowmushroom => YELLOW_MUSHROOM_PREFAB_NAME,
            PlantType.purplemushroom => PURPLE_MUSHROOM_PREFAB_NAME,
            _ => " "
        };

        if(!CheckNeeded(type) || spawnObjName == " ") { return; }

        var obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",spawnObjName), itemDropPos.position, new Quaternion(Random.Range(0,359),Random.Range(0,359),Random.Range(0,359), 1));
        plantObjs.Add(obj);
        SoundManager.Instance.RPCPlaySound3D("Spawn PlantObj", itemDropPos.position);

        StartCoroutine(DecreaseNeededItemAmount(type));
    }

    public void CollectCookedItem(Inventory inventory)
    {
        inventory.CollectItem(PlantType.potion, GetComponent<PlantObj>());
        PV.RPC(nameof(RPC_CollectedCookedItem),RpcTarget.All);
    }

    IEnumerator DecreaseNeededItemAmount(PlantType type)
    {
        yield return new WaitForSeconds(decreaseTime);

        PV.RPC(nameof(UpdateNeededItemAmount), RpcTarget.All, type);

        StopCoroutine(DecreaseNeededItemAmount(type));
    }

    [PunRPC] void UpdateNeededItemAmount(PlantType type)
    {
        if(type == PlantType.daffodil)
        {
            currentDaffodil--;
            daffodilText.text = currentDaffodil.ToString();
            if(currentDaffodil <= 0)
                daffodilTaskImage.gameObject.SetActive(false);
        }
        else if(type == PlantType.hyacinth)
        {
            currentHyacinth--;
            hycainthText.text = currentHyacinth.ToString();
            if(currentHyacinth <= 0)
                hyacinthTaskImage.gameObject.SetActive(false);
        }
        else if(type == PlantType.yellowmushroom)
        {
            currentYellowMushroom--;
            yellowMushroomText.text = currentYellowMushroom.ToString();
            if(currentYellowMushroom <= 0)
                yellowMushroomTaskImage.gameObject.SetActive(false);
        }
        else if(type == PlantType.purplemushroom)
        {
            currentPurpleMushroom--;
            purpleMushroomText.text = currentPurpleMushroom.ToString();
            if(currentPurpleMushroom <= 0)
                purpleMushroomTaskImage.gameObject.SetActive(false);
        }

        if(PhotonNetwork.IsMasterClient)
        {
            if(CheckCanCook())
            {
                PV.RPC(nameof(RPC_Cook), RpcTarget.All);
            }
        }
    }

    bool CheckNeeded(PlantType type)
    {
        int neededAmount = type switch
        {
            PlantType.daffodil => currentDaffodil,
            PlantType.hyacinth => currentHyacinth,
            PlantType.yellowmushroom => currentYellowMushroom,
            _ => currentPurpleMushroom
        };

        if(neededAmount > 0)
            return true;
        else
            return false;
    }

    bool CheckCanCook()
    {
        if(currentDaffodil > 0)
            return false;
        else if(currentHyacinth > 0)
            return false;
        else if(currentYellowMushroom > 0)
            return false;
        else if(currentPurpleMushroom > 0)
            return false;
        else
            return true;
    }

    [PunRPC] void RPC_Cook()
    {
        GetComponent<Animator>().SetTrigger("Cook");
        liquid.SetActive(true);
        myLight.SetActive(true);
        cookingVFX.Play();
        boilingSFX.Play();
        cookingSliderBG.SetActive(true);

        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(CookRoutine());
        }
    }

    IEnumerator CookRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(updateSliderValueTime);
            PV.RPC(nameof(RPC_UpdateSliderValue),RpcTarget.All);
            if(cookingSlider.fillAmount >= 1)
            {
                break;
            }
        }

        DestroyAllPlantObj();
        PV.RPC(nameof(RPC_Cooked),RpcTarget.AllViaServer);
    }

    [PunRPC] void RPC_CollectedCookedItem()
    {
        daffodilTaskImage.gameObject.SetActive(true);
        hyacinthTaskImage.gameObject.SetActive(true);
        yellowMushroomTaskImage.gameObject.SetActive(true);
        purpleMushroomTaskImage.gameObject.SetActive(true);

        readyItem.SetActive(false);
        itemReady = false;

        currentDaffodil = maxDaffodil;
        currentHyacinth = maxHyacinth;
        currentYellowMushroom = maxYellowMushroom;
        currentPurpleMushroom = maxPurpleMushroom;

        daffodilText.text = currentDaffodil.ToString();
        hycainthText.text = currentHyacinth.ToString();
        yellowMushroomText.text = currentYellowMushroom.ToString();
        purpleMushroomText.text = currentPurpleMushroom.ToString();

        cookingSliderCurrentTime = 0;
        cookingSlider.fillAmount = cookingSliderCurrentTime;
        cookingSliderBG.SetActive(false);
        liquid.SetActive(false);
        myLight.SetActive(false);
    }

    [PunRPC] void RPC_UpdateSliderValue()
    {
        cookingSliderCurrentTime += 1 / cookTime;
        cookingSlider.fillAmount = cookingSliderCurrentTime;
    }

    [PunRPC] void RPC_Cooked()
    {
        readyItem.SetActive(true);
        itemReady = true;
        GetComponent<Animator>().SetTrigger("Idle");
        myLight.SetActive(false);
        cookingVFX.Stop();
        boilingSFX.Stop();
        cookingSliderBG.SetActive(false);
    }

    void DestroyAllPlantObj()
    {
        foreach (var item in plantObjs)
        {
            PhotonNetwork.Destroy(item);
        }
        plantObjs.Clear();
    }

}
