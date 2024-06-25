using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class AbilityLightning : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI lightningCDText;
    [SerializeField] Image lightningBG;

    [SerializeField] Camera cam;

    [SerializeField] GameObject lightningPrefab;

    [SerializeField] float manaCost;

    Vector3 startPos;
    Vector3 endPos;

    [SerializeField] int abilityCD;
    bool canUseLightning = true;
    int currentAbilityCD;

    public float GetManaCost
    {
        get
        {
            return manaCost;
        }
    }

    public bool GetCanUseLightning
    {
        get
        {
            return canUseLightning;
        }
    }

    void Start() 
    {
        if(!IsOwner) enabled = false;

        DisableLightningCDCounter();
    }

    public void UseLightning()
    {
        Mana.Instance.DecreaseMana(manaCost);

        Ray ray = cam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity))
        {
            UseLightningServerRpc(hitData.point); // sync
        }
        
        StartCoroutine(UseLightningCo()); // cd
    }

    [ServerRpc(RequireOwnership = false)] void UseLightningServerRpc(Vector3 dir)
    {
        var lightning = Instantiate(lightningPrefab, dir, Quaternion.identity);
        lightning.GetComponent<NetworkObject>().SpawnWithOwnership(NetworkManager.ServerClientId, true);
        lightning.GetComponent<LightningBoltScript>().SetSpawnValues();
    }

    IEnumerator UseLightningCo()
    {
        Mana.Instance.DecreaseMana(GetManaCost);
        canUseLightning = false;

        currentAbilityCD = abilityCD;
        EnableLightningCDCounter();
        SetLightningCDText();

        for (int i = 0; i < abilityCD; i++)
        {   
            yield return new WaitForSeconds(1);
            currentAbilityCD--;
            
            SetLightningCDText();
        }
       
        canUseLightning = true;
        StopCoroutine(UseLightningCo());
    }

    void SetLightningCDText()
    {
        lightningCDText.text = currentAbilityCD.ToString();
        if(currentAbilityCD == 0)
        {
            DisableLightningCDCounter();
        }
    }

    void EnableLightningCDCounter()
    {
        lightningCDText.enabled = true;
        lightningBG.enabled = true;
    }

    void DisableLightningCDCounter()
    {
        lightningCDText.enabled = false;
        lightningBG.enabled = false;
    }

}
