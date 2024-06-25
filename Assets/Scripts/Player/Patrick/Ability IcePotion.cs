using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AbilityIcePotion : NetworkBehaviour
{
    [SerializeField] Rigidbody myRigidbody;
    [SerializeField] GameObject IceFX;

    [SerializeField] float rotateSpeed;

    Vector3 rotateVector;
    Transform player;

    public Transform SetPlayer
    {
        set
        {
            player = value;
        }
    }

    void Start() 
    {
        if(!IsOwner) enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        rotateVector = Random.insideUnitSphere.normalized;
    }

    void FixedUpdate() 
    {
        transform.Rotate(rotateVector * rotateSpeed);
    }

    public void ThrowPotion(Vector3 direction)
    {
        myRigidbody.AddForce(direction,ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision other) 
    {
        Vector3 dir = transform.forward;
        dir = -(transform.position - player.position).normalized;
        
        dir.y = 0;
        
        SoundManager.Instance.PlaySound3D("Break Glass",transform.position);
        
        SpawnIceVFX(dir);
        
        KYSPotionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] void KYSPotionServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }

    void SpawnIceVFX(Vector3 direction)
    {
        SpawnIceVFXServerRpc(direction);
    }

    [ServerRpc(RequireOwnership = false)] void SpawnIceVFXServerRpc(Vector3 direction)
    {
        var iceVFX = Instantiate(IceFX, transform.position + direction * 1, Quaternion.identity);
        iceVFX.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
    }

}
