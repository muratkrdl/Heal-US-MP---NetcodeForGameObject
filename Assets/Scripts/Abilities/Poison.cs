using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Poison : NetworkBehaviour
{
    [SerializeField] ParticleSystem poisonVFX;

    public void OpenPoison()
    {
        ChangePoisonStateServerRpc(true);
    }

    public void ClosePoison()
    {
        ChangePoisonStateServerRpc(false);
    }

    [ServerRpc] void ChangePoisonStateServerRpc(bool _state)
    {
        ChangePoisonStateClientRpc(_state);
    }

    [ClientRpc] void ChangePoisonStateClientRpc(bool _state)
    {
        if(_state)
        {
            poisonVFX.Play();
        }
        else
        {
            poisonVFX.Stop();
        }
    }

}
