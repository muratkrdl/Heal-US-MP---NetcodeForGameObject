using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DoctorCollisionParticle : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] ParticleSystem poisonFX;

    [SerializeField] int poisonDamageCounter;
    [SerializeField] int poisonDamageDecreaseComplierAmount;

    [SerializeField] PlayerHP playerHP;

    bool isPoisened = false;

    int poisonComplier;

    public int PoisonDamage
    {
        get
        {
            return poisonDamageCounter;
        }
        set
        {
            poisonDamageCounter = value;
        }
    }

    void OnParticleCollision(GameObject other) 
    {
        if(!PV.IsMine) { return; }
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
        PV.RPC(nameof(RPC_OpenPoisonVFX), RpcTarget.All);
        yield return new WaitForSeconds(2.85f);
        int damage = Mathf.RoundToInt(poisonComplier / poisonDamageDecreaseComplierAmount);
        for (int i = 0; i < poisonDamageCounter; i++)
        {
            playerHP.DecreaseHP(damage);
            SoundManager.Instance.RPCPlaySound3D("Poison Hit", transform.position);
            Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
            yield return new WaitForSeconds(1);
        }
        PV.RPC(nameof(RPC_ClosePoisonVFX), RpcTarget.All);
        poisonComplier = 0;
        isPoisened = false;
        StopCoroutine(GetDamageFromPoison());
    }

    [PunRPC] void RPC_ClosePoisonVFX()
    {
        poisonFX.Stop();
    }

    [PunRPC] void RPC_OpenPoisonVFX()
    {
        poisonFX.Play();
    }

}
