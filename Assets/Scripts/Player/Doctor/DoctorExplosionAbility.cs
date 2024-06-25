using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DoctorExplosionAbility : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI highExplosionCDText;
    [SerializeField] Image highExplosionBG;

    [SerializeField] GameObject energyExplosionPrefab;
    [SerializeField] Camera cam;

    [SerializeField] int abilityCD;

    [SerializeField] float manaCost;

    int currentAbilityCD = 0;

    bool canUseHighExplosion = true;

    public float GetManaCost
    {
        get
        {
            return manaCost;
        }
    }

    public bool GetCanUseHighExplosion
    {
        get
        {
            return canUseHighExplosion;
        }
    }

    void Start() 
    {
        DisableHighExplosionCDCounter();
    }

    public void UseHighExplosion()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hitData,Mathf.Infinity))
        {
            if(hitData.transform.CompareTag("Lever") ||
            hitData.transform.CompareTag("Key") ||
            hitData.transform.CompareTag("Gate")) return;

            Mana.Instance.DecreaseMana(manaCost);
            HighExplosion(hitData.point);
        }
    }

    void HighExplosion(Vector3 pos)
    {
        HighExplosionServerRpc(pos);

        StartCoroutine(UseHighExplosionCo());
    }

    [ServerRpc(RequireOwnership = false)] void HighExplosionServerRpc(Vector3 pos)
    {
        Instantiate(energyExplosionPrefab, pos, Quaternion.identity).GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
    }

    IEnumerator UseHighExplosionCo()
    {
        canUseHighExplosion = false;

        currentAbilityCD = abilityCD;
        EnableHighExplosionCDCounter();
        SetHighExplosionCDText();
      
        for (int i = 0; i < abilityCD; i++)
        {   
            yield return new WaitForSeconds(1);
            currentAbilityCD--;
            SetHighExplosionCDText();
        }
       
        canUseHighExplosion = true;
        StopCoroutine(UseHighExplosionCo());
    }

    void SetHighExplosionCDText()
    {
        highExplosionCDText.text = currentAbilityCD.ToString();
        if(currentAbilityCD == 0)
        {
            DisableHighExplosionCDCounter();
        }
    }

    void EnableHighExplosionCDCounter()
    {
        highExplosionCDText.enabled = true;
        highExplosionBG.enabled = true;
    }

    void DisableHighExplosionCDCounter()
    {
        highExplosionCDText.enabled = false;
        highExplosionBG.enabled = false;
    }

}
