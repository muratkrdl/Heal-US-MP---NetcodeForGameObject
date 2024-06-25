using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoctorCollisionParticle : NetworkBehaviour
{
    [SerializeField] ParticleSystem poisonFX;

    [SerializeField] int poisonDamageCounter;
    [SerializeField] int poisonDamageDecreaseComplierAmount;

    [SerializeField] PlayerHP playerHP;

    bool isPoisened = false;

    int poisonComplier;

    void Start() 
    {
        if(!IsOwner) enabled = false;    
    }

    void OnParticleCollision(GameObject other) 
    {
        poisonComplier++;
        if(!isPoisened)
        {
            StartCoroutine(GetDamageFromPoison());
        }
    }

    public void StartGetDamageFromPoison(int value)
    {
        poisonComplier = value;
        StartCoroutine(GetDamageFromPoison());
    }

    IEnumerator GetDamageFromPoison()
    {
        isPoisened = true;

        ChangePoisonVFXStateServerRpc(true);

        yield return new WaitForSeconds(2.85f);
        int damage = Mathf.RoundToInt(poisonComplier / poisonDamageDecreaseComplierAmount);
        for (int i = 0; i < poisonDamageCounter; i++)
        {
            playerHP.DecreaseHP(damage);
            SoundManager.Instance.PlaySound3D("Poison Hit", transform.position); // herkese çal
            Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
            yield return new WaitForSeconds(1);
        }

        ChangePoisonVFXStateServerRpc(false);

        poisonComplier = 0;
        isPoisened = false;
        StopCoroutine(GetDamageFromPoison());
    }

    [ServerRpc(RequireOwnership = false)] void ChangePoisonVFXStateServerRpc(bool _state)
    {
        ChangePoisonVFXStateClientRpc(_state);
    }

    [ClientRpc] void ChangePoisonVFXStateClientRpc(bool _state)
    {
        if(_state)
        {
            poisonFX.Play();
        }
        else
        {
            poisonFX.Stop();
        }
    }

}
