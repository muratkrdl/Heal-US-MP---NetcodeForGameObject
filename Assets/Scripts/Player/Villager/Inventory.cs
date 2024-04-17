using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] InventorySlot[] inventorySlots;

    int choosenIndex;

    void Start() 
    {
        UpdateChoosenItem(0);
    }

    void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            UpdateChoosenItem(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            UpdateChoosenItem(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            UpdateChoosenItem(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            UpdateChoosenItem(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            UpdateChoosenItem(4);
        }
    }

    void UpdateChoosenItem(int index)
    {
        choosenIndex = index;
        foreach (var item in inventorySlots)
        {
            item.SetVisualArea(false);
        }
        inventorySlots[choosenIndex].SetVisualArea(true);
    }

    public void CollectItem(PlantType type, PlantObj destroyObj)
    {
        if(ContainThisType(type))
        {
            foreach(var item in inventorySlots)
            {
                if(item.GetItemType == type)
                {
                    item.IncreaseItemAmount();
                    break;
                }
            }
        }
        else if(!CheckHasItem())
        {
            inventorySlots[choosenIndex].SetItem(type);
        }
        else if(inventorySlots[choosenIndex].GetItemType == type)
        {
            inventorySlots[choosenIndex].IncreaseItemAmount();
        }
        else if(FindEmptySlot() != null)
        {
            FindEmptySlot().SetItem(type);
        }
        else
        {
            return;
        }

        if(type == PlantType.potion)
        {
            return;
        }

        destroyObj.KYS();
    }

    bool ContainThisType(PlantType type)
    {
        foreach (var item in inventorySlots)
        {
            if(item.GetItemType == type)
            {
                return true;
            }
        }
        
        return false;
    }

    public int GetCurrentItemAmount()
    {
        if(!inventorySlots[choosenIndex].GetHasItem) { return -1; }
        return inventorySlots[choosenIndex].GetItemAmount;
    }

    public void DecreaseItemAmount()
    {
        inventorySlots[choosenIndex].DecreaseItemAmount();
    }

    public PlantType GetItemType()
    {
        return inventorySlots[choosenIndex].GetItemType;
    }

    bool CheckHasItem()
    {
        return inventorySlots[choosenIndex].GetHasItem || inventorySlots[choosenIndex].GetItemAmount > 0;
    }

    public bool CanUseFlash()
    {
        if(inventorySlots[choosenIndex].GetHasItem && inventorySlots[choosenIndex].GetItemAmount > 0)
        {
            return true;
        }
        
        return false;
    }

    InventorySlot FindEmptySlot()
    {
        foreach (var item in inventorySlots)
        {
            if(!item.GetHasItem && item.GetItemAmount <= 0)
            {
                return item;
            }
        }

        return null;
    }

}
