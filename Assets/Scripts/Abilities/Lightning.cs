using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lightning : NetworkBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float stunTime;

    [SerializeField] BoxCollider boxCollider;

    [SerializeField] float damage;

    void Start() 
    {
        SoundManager.Instance.PlaySound3D("Lightning",transform.position);
        if(!IsOwner) return;
        Invoke(nameof(DisableLightning),lifeTime);
    }

    void DisableLightning()
    {
        DisableLightningClientRpc();

        Invoke(nameof(KYS),lifeTime*4);
    }

    [ClientRpc] void DisableLightningClientRpc()
    {
        GetComponent<LineRenderer>().enabled = false;
        boxCollider.enabled = false;
    }

    void KYS()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Doctor"))
        {
            boxCollider.enabled = false;
            if(other.TryGetComponent(out Doctor doctor))
            {
                Manager.Instance.SpawnFloatingText(doctor.transform.position, damage.ToString(), Color.red);
                doctor.MonsterStunned(stunTime,true);
                doctor.GetComponentInChildren<PlayerHP>().DecreaseHP(damage);
            }
        }
        else if(other.TryGetComponent<StoneMonster>(out var stoneMonster))
        {
            stoneMonster.LifeTimeKYS();
        }
    }

}
