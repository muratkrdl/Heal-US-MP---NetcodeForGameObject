using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StoneMonster : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] ParticleSystem tailFX;
    [SerializeField] ParticleSystem dieFX;

    [SerializeField] SkinnedMeshRenderer myRenderer;

    [SerializeField] BoxCollider boxCollider;

    float damage;

    float lifeTime;

    Transform target;

    public Transform SetTarget
    {
        set
        {
            target = value;
        }
    }

    public float SetDamage
    {
        set
        {
            damage = value;
        }
    }

    public float SetLifeTime
    {
        set
        {
            lifeTime = value;
        }
    }

    void Start() 
    {
        transform.LookAt(target);
        Invoke(nameof(LifeTimeKYS),lifeTime);
        StartCoroutine(StartDestination());
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
        myRenderer.enabled = false;
        boxCollider.enabled = false;
        tailFX.Stop();
        dieFX.Play();
        Invoke(nameof(KYS),1);
    }

    void KYS()
    {
        SoundManager.Instance.RPCPlaySound3D("Stone Monster Die", transform.position);
        Destroy(gameObject);
    }

}
