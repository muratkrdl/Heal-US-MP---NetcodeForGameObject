using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class StoneMonster : NetworkBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] ParticleSystem tailFX;
    [SerializeField] ParticleSystem dieFX;

    [SerializeField] SkinnedMeshRenderer myRenderer;

    [SerializeField] BoxCollider boxCollider;

    [SerializeField] float damage;

    float lifeTime;

    Transform target;

    void Start() 
    {
        transform.LookAt(target);
        if(!IsOwner) return;
        Invoke(nameof(LifeTimeKYS),lifeTime);
        StartCoroutine(StartDestination());
    }

    public void SetSpawnValues(Transform _target, float _lifeTime, Vector3 _pos)
    {
        target = _target;
        lifeTime = _lifeTime;
        transform.position = _pos;
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Patrick"))
        {
            other.gameObject.GetComponentInChildren<PlayerHP>().DecreaseHP(damage);
            Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
            StopAllCoroutines();
            LifeTimeKYS();
        }
    }

    IEnumerator StartDestination()
    {
        while(true)
        {
            yield return null;
            navMeshAgent.destination = target.position;
        }
    }

    public void LifeTimeKYS()
    {
        LifeTimeKYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void LifeTimeKYSServerRpc()
    {
        LifeTimeKYSClientRpc();
    }

    [ClientRpc] void LifeTimeKYSClientRpc()
    {
        myRenderer.enabled = false;
        boxCollider.enabled = false;
        tailFX.Stop();
        dieFX.Play();

        if(!IsOwner) return;
        Invoke(nameof(KYS), 2);
    }

    void KYS()
    {
        KYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        SoundManager.Instance.PlaySound3D("Stone Monster Die", transform.position); // herkese çal
        GetComponent<NetworkObject>().Despawn();
    }

}
