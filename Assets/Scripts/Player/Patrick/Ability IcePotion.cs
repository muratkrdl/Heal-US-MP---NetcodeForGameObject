using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AbilityIcePotion : MonoBehaviour
{
    [SerializeField] PhotonView PV;

    [SerializeField] Rigidbody myRigidbody;
    [SerializeField] GameObject IceFX;

    [SerializeField] float rotateSpeed;

    int slowAmount;
    Vector3 rotateVector;
    Transform player;

    public Transform SetPlayer
    {
        set
        {
            player = value;
        }
    }

    public int SetSlowAmount
    {
        set
        {
            slowAmount = value;
        }
    }

    void Start() 
    {
        rotateVector = Random.insideUnitSphere.normalized;
    }

    void FixedUpdate() 
    {
        if(!PV.IsMine) { return; }
        transform.Rotate(rotateVector * rotateSpeed);
    }

    public void ThrowPotion(Vector3 direction)
    {
        myRigidbody.AddForce(direction,ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other) 
    {
        if(!PV.IsMine) { return; }

        Vector3 dir = transform.forward;
        dir = -(transform.position - player.position).normalized;
        
        dir.y = 0;
        SoundManager.Instance.PlaySound3D("Break Glass",transform.position);
        PV.RPC(nameof(SpawnIceVFX),RpcTarget.All,dir);
        
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC] void SpawnIceVFX(Vector3 direction)
    {
        var iceVFX = Instantiate(IceFX,transform.position,Quaternion.identity);
        iceVFX.transform.position += direction * 1;
        IceFX.GetComponent<Ice>().SetSlowAmount = slowAmount;
        IceFX.GetComponent<Ice>().SetPercentSlowAmount();
    }

}
