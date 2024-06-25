using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class AbilityFireball : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fireballCDText;
    [SerializeField] Image fireballBG;

    [SerializeField] Camera cam;

    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPosition;

    [SerializeField] Transform mouseLookPos;

    [SerializeField] int abilityCD;

    [SerializeField] float manaCost;

    int currentAbilityCD = 0;

    bool canThrowball = true;
    
    public float GetManaCost
    {
        get
        {
            return manaCost;
        }
    }

    public bool GetCanThrowFireball
    {
        get
        {
            return canThrowball;
        }
    }

    void Start() 
    {
        DisableFireballCDCounter();
    }

    public void ThrowFireBall()
    {
        Mana.Instance.DecreaseMana(manaCost);
        
        var direction = (mouseLookPos.position - cam.transform.position).normalized;

        ThrowBall(direction); // sync
    }

    void ThrowBall(Vector3 dir)
    {
        ThrowBallServerRpc(dir);

        if(!GetComponentInParent<NetworkObject>().IsOwner) return;

        StartCoroutine(ThrowFireBallCo()); // cd
    }

    [ServerRpc(RequireOwnership = false)] void ThrowBallServerRpc(Vector3 dir)
    {
        var fireball = Instantiate(fireballPrefab, fireballSpawnPosition.position, Quaternion.identity);
        fireball.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
        fireball.GetComponent<FireBall>().ThrowBall(dir);
    }

    IEnumerator ThrowFireBallCo()
    {
        canThrowball = false;

        currentAbilityCD = abilityCD;
        EnableFireballCDCounter();
        SetFireBallCDText();
      
        for (int i = 0; i < abilityCD; i++)
        {   
            yield return new WaitForSeconds(1);
            currentAbilityCD--;
            SetFireBallCDText();
        }
       
        canThrowball = true;
        StopCoroutine(ThrowFireBallCo());
    }

    void SetFireBallCDText()
    {
        fireballCDText.text = currentAbilityCD.ToString();
        if(currentAbilityCD == 0)
        {
            DisableFireballCDCounter();
        }
    }

    void EnableFireballCDCounter()
    {
        fireballCDText.enabled = true;
        fireballBG.enabled = true;
    }

    void DisableFireballCDCounter()
    {
        fireballCDText.enabled = false;
        fireballBG.enabled = false;
    }

}
