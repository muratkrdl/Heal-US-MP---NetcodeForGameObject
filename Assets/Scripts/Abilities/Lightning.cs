using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float stunTime;

    [SerializeField] BoxCollider boxCollider;

    float damage;

    public float SetDamage
    {
        set
        {
            damage = value;
        }
    }

    void Start() 
    {
        SoundManager.Instance.PlaySound3D("Lightning",transform.position);
        Invoke(nameof(DisableLightning),lifeTime);
    }

    void DisableLightning()
    {
        GetComponent<LineRenderer>().enabled = false;
        boxCollider.enabled = false;
        Invoke(nameof(KYS),lifeTime*4);
    }

    void KYS()
    {
        Destroy(gameObject);
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
