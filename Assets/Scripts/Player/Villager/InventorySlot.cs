using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] GameObject choosenVisualArea;

    [SerializeField] GameObject inventoryItemPrefab;

    [SerializeField] Sprite daffodilSprite;
    [SerializeField] Sprite hyacinthSprite;
    [SerializeField] Sprite yellowMushroomSprite;
    [SerializeField] Sprite purpleMushroomSprite;
    [SerializeField] Sprite cookedPotionSprite;

    GameObject inventoryItem;

    bool hasItem;

    PlantType itemType = PlantType.none;
    int itemAmount = 0;

    public PlantType GetItemType
    {
        get
        {
            return itemType;
        }
    }

    public int GetItemAmount
    {
        get
        {
            return itemAmount;
        }
    }

    public bool GetHasItem
    {
        get
        {
            return hasItem;
        }
    }

    public void IncreaseItemAmount()
    {
        itemAmount++;
        inventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = itemAmount.ToString();
    }

    public void DecreaseItemAmount()
    {
        itemAmount--;
        inventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = itemAmount.ToString();
        if(itemAmount <= 0)
        {
            itemType = PlantType.none;
            hasItem = false;
            Destroy(inventoryItem);
        }
    }

    public void SetItem(PlantType type)
    {
        Sprite choosenSprite = type switch
        {
            PlantType.daffodil => daffodilSprite,
            PlantType.hyacinth => hyacinthSprite,
            PlantType.yellowmushroom => yellowMushroomSprite,
            PlantType.purplemushroom => purpleMushroomSprite,
            _ => cookedPotionSprite
        };

        inventoryItem = Instantiate(inventoryItemPrefab,transform);
        inventoryItem.GetComponent<Image>().sprite = choosenSprite;
        inventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = itemAmount.ToString();

        hasItem = true;
        itemType = type;
        IncreaseItemAmount();
    }

    public void SetVisualArea(bool value)
    {
        choosenVisualArea.SetActive(value);
    }

}
