using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] Camera playerCam;

    [SerializeField] float range;
    [SerializeField] LayerMask layerMask;

    [SerializeField] KeyCode keyCode;

    bool hasKey;

    public bool HasKey
    {
        get
        {
            return hasKey;
        }
        set
        {
            hasKey = value;
        }
    }

    void Start() 
    {
        hasKey = false;
    }

    void Update()
    {
        if(!PV.IsMine) { return; }

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
            if(hit.transform.TryGetComponent<IInteractable>(out var obj))
            {
                obj.Interact();
            }
            else if(hit.transform.TryGetComponent<IBoolInteractable>(out var objj))
            {
                objj.Interact(this);
            }
        }
    }
    
}
