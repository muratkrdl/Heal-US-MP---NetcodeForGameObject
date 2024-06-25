using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VillagerInteract : NetworkBehaviour
{
    [SerializeField] Camera playerCam;

    [SerializeField] float range;
    [SerializeField] LayerMask layerMask;

    [SerializeField] KeyCode keyCode;

    [SerializeField] Inventory inventory;

    void Start() 
    {
        if(!IsOwner) enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(keyCode))
        {
            InteractObj();
        }
    }

    void InteractObj()
    {
        Ray ray = playerCam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = playerCam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            if(hit.transform.CompareTag("Cauldron"))
            {
                if(inventory.GetCurrentItemAmount() > 0 && !hit.transform.GetComponentInParent<Cauldron>().GetItemReady && inventory.GetItemType() != PlantType.potion) 
                { 
                    hit.transform.GetComponentInParent<Cauldron>().InteractWithCauldron(inventory.GetItemType());
                    inventory.DecreaseItemAmount();
                    return;
                }
                else if(hit.transform.GetComponentInParent<Cauldron>().GetItemReady)
                {
                    hit.transform.GetComponentInParent<Cauldron>().CollectCookedItem(inventory);
                    SoundManager.Instance.PlaySound3D("Pick Up", transform.position);
                    return; 
                }
            }
            else if(hit.transform.TryGetComponent<PlantObj>(out var plantObj))
            {
                PlantType plantType = hit.transform.tag switch
                {
                    "daffodil" => PlantType.daffodil,
                    "hyacinth" => PlantType.hyacinth,
                    "yellowMushroom" => PlantType.yellowmushroom,
                    _ => PlantType.purplemushroom
                };
                
                if(plantObj != null)
                {
                    inventory.CollectItem(plantType, plantObj);
                    SoundManager.Instance.PlaySound3D("Collect Plant", transform.position);
                }
            }
        }
    }
}
