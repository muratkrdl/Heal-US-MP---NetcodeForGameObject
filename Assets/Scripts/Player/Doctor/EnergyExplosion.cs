using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnergyExplosion : NetworkBehaviour
{
    [SerializeField] float damage;

    void Start() 
    {
        if(!IsOwner) return;
        Invoke(nameof(KYSServerRpc), 2);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Patrick"))
        {
            other.gameObject.GetComponentInChildren<PlayerHP>().DecreaseHP(damage);
            Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
        }
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

}
