using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Netcode;
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

public class Cauldron : NetworkBehaviour
{
    [SerializeField] GameObject[] spawnObjPrefabs;

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
        int spawnObjIndex = type switch
        {
            PlantType.daffodil => 0,
            PlantType.hyacinth => 1,
            PlantType.yellowmushroom => 2,
            PlantType.purplemushroom => 3,
            _ => 4
        };

        if(!CheckNeeded(type)) { return; }
        SpawnObjServerRpc(spawnObjIndex);
        
        SoundManager.Instance.PlaySound3D("Spawn PlantObj", itemDropPos.position); // herkese çal

        StartCoroutine(DecreaseNeededItemAmount(type));
    }

    [ServerRpc(RequireOwnership = false)] void SpawnObjServerRpc(int index)
    {
        var spawnObj = spawnObjPrefabs[index];
        var spawnedObj = Instantiate(spawnObj, itemDropPos);
        spawnedObj.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
        plantObjs.Add(spawnedObj);
    }

    public void CollectCookedItem(Inventory inventory)
    {
        inventory.CollectItem(PlantType.potion, GetComponent<PlantObj>());
        CollectedCookedItemServerRpc();
    }

    IEnumerator DecreaseNeededItemAmount(PlantType type)
    {
        yield return new WaitForSeconds(decreaseTime);

        UpdateNeededItemAmount(type);

        StopCoroutine(DecreaseNeededItemAmount(type));
    }

    void UpdateNeededItemAmount(PlantType type)
    {
        UpdateNeededItemAmountServerRpc(type);
    }

    [ServerRpc(RequireOwnership = false)] void UpdateNeededItemAmountServerRpc(PlantType type)
    {
        UpdateNeededItemAmountClientRpc(type);
    }

    [ClientRpc] void UpdateNeededItemAmountClientRpc(PlantType type)
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

        if(CheckCanCook())
        {
            if(!IsOwner) return;
            CookServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)] void CookServerRpc()
    {
        StartCookClientRpc();

        if(IsServer)
        {
            StartCoroutine(CookRoutine());
        }
    }

    [ClientRpc] void StartCookClientRpc()
    {
        GetComponent<Animator>().SetTrigger("Cook");
        liquid.SetActive(true);
        myLight.SetActive(true);
        cookingVFX.Play();
        boilingSFX.Play();
        cookingSliderBG.SetActive(true);
    }

    IEnumerator CookRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(updateSliderValueTime);
            cookingSliderCurrentTime += 1 / cookTime;
            UpdateSliderValueClientRpc(cookingSliderCurrentTime);
            if(cookingSlider.fillAmount >= 1)
            {
                break;
            }
        }

        DestroyAllPlantObj();
        CookedClientRpc();
        StopCoroutine(CookRoutine());
    }

    [ServerRpc(RequireOwnership = false)] void CollectedCookedItemServerRpc()
    {
        CollectedCookedItemClientRpc();
    }

    [ClientRpc] void CollectedCookedItemClientRpc()
    {
        readyItem.SetActive(false);
        itemReady = false;

        daffodilTaskImage.gameObject.SetActive(true);
        hyacinthTaskImage.gameObject.SetActive(true);
        yellowMushroomTaskImage.gameObject.SetActive(true);
        purpleMushroomTaskImage.gameObject.SetActive(true);

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

    [ClientRpc] void CookedClientRpc()
    {
        readyItem.SetActive(true);
        itemReady = true;
        GetComponent<Animator>().SetTrigger("Idle");
        myLight.SetActive(false);
        cookingVFX.Stop();
        boilingSFX.Stop();
        cookingSliderBG.SetActive(false);
    }

    [ClientRpc] void UpdateSliderValueClientRpc(float fillAmount)
    {
        cookingSlider.fillAmount = fillAmount;
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

    void DestroyAllPlantObj()
    {
        DestroyAllPlantObjServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void DestroyAllPlantObjServerRpc()
    {
        foreach (var item in plantObjs)
        {
            item.GetComponent<NetworkObject>().Despawn();
        }

        plantObjs.Clear();
    }

}
