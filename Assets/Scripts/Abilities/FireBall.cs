using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FireBall : NetworkBehaviour
{
    [SerializeField] Rigidbody myRigidbody;

    [SerializeField] float speed;
    [SerializeField] float lifeTime;
    [SerializeField] float lerpTime;

    [SerializeField] ParticleSystem[] efects;
    [SerializeField] Light ponitLight;
    [SerializeField] GameObject sphere;

    [SerializeField] int stunTime;
    [SerializeField] float damage;

    void Start() 
    {
        Invoke(nameof(DeactiveBall),lifeTime);

        if(!IsOwner) return;
        myRigidbody.AddForce(transform.forward * speed);
        Invoke(nameof(KYS),lifeTime+2);
    }
    
    public void ThrowBall(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
    }

    void DeactiveBall()
    {
        DisableBall();
    }

    void DisableBall()
    {
        StartCoroutine(AboutSphere());
        GetComponent<SphereCollider>().enabled = false;
        DisableFX();
    }

    IEnumerator AboutSphere()
    {
        while(true)
        {
            yield return null;
            sphere.transform.localScale = Vector3.Lerp(sphere.transform.localScale,Vector3.zero, lerpTime * Time.deltaTime);
            ponitLight.range = Mathf.Lerp(ponitLight.range,0,lerpTime * Time.deltaTime);
            if(sphere.transform.localScale.x <= .1f)
            {
                sphere.SetActive(false);
                break;
            }
        }
        StopCoroutine(AboutSphere());
    }

    void KYS()
    {
        KYSServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
    }

    void DisableFX()
    {
        ponitLight.enabled = false;
        foreach (var item in efects)
        {
            item.Stop();
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Doctor"))
        {
            StopAllCoroutines();

            if(other.gameObject.TryGetComponent<Doctor>(out var doctor))
            {
                Manager.Instance.SpawnFloatingText(doctor.transform.position, damage.ToString(), Color.red);
                doctor.MonsterStunned(stunTime,false);
                doctor.GetComponentInChildren<PlayerHP>().DecreaseHP(damage);
            }
            else if(other.TryGetComponent<StoneMonster>(out var stoneMonster))
            {
                stoneMonster.LifeTimeKYS();
            }
            DisableBall();
            Invoke(nameof(KYS),2);
        }
    }

}
