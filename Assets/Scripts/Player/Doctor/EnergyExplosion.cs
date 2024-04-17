using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyExplosion : MonoBehaviour
{

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
        Destroy(gameObject,2);
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Patrick"))
        {
            other.gameObject.GetComponentInChildren<PlayerHP>().DecreaseHP(damage);
            Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
        }
    }

}
