using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] Rigidbody myRigidbody;

    [SerializeField] float speed;
    [SerializeField] float lifeTime;
    [SerializeField] float lerpTime;

    [SerializeField] ParticleSystem[] efects;
    [SerializeField] Light ponitLight;
    [SerializeField] GameObject sphere;

    int stunTime;
    float damage;

    public int SetStunTime
    {
        set
        {
            stunTime = value;
        }
    }

    public float SetFireBallDamage
    {
        set
        {
            damage = value;
        }
    }

    void Start() 
    {
        Invoke(nameof(DeactiveBall),lifeTime);
        Invoke(nameof(KYS),lifeTime+2);
    }
    
    public void ThrowBall(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        myRigidbody.AddForce(direction * speed);
    }

    void DeactiveBall()
    {
        DisableBall();
    }

    void DisableBall()
    {
        StartCoroutine(AboutSphere());
        GetComponent<SphereCollider>().enabled = false;
        myRigidbody.velocity = Vector3.zero;
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
        Destroy(gameObject);
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
