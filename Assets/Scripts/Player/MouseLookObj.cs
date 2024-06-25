using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLookObj : NetworkBehaviour
{
    [SerializeField] MouseLook mouseLook;

    void Update()
    {
        if(IsClient && IsOwner)
		{
			mouseLook.ProcessLocalPlayerMovement();
		}
		if(!IsOwner)
		{
			mouseLook.ProcessSimulatedPlayerMovement();
		}
    }

}
