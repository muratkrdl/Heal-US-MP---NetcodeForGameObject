using System.Collections;
using System.Collections.Generic;
using DigitalRuby.LightningBolt;
using UnityEngine;
using Photon.Pun;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class AbilityLightning : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lightningCDText;
    [SerializeField] Image lightningBG;

    [SerializeField] PhotonView PV;

    [SerializeField] Camera cam;

    [SerializeField] GameObject lightningPrefab;

    [SerializeField] float damage;
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
        DisableLightningCDCounter();
    }

    public void UseLightning()
    {
        Mana.Instance.DecreaseMana(manaCost);

        Ray ray = cam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hitData, Mathf.Infinity))
        {
            PV.RPC(nameof(SpawnLightning),RpcTarget.All,hitData.point);
        }
    }

    [PunRPC] void SpawnLightning(Vector3 mousePos)
    {
        var lightning = Instantiate(lightningPrefab,mousePos,Quaternion.identity);

        lightning.GetComponent<Lightning>().SetDamage = damage;
        endPos = mousePos + new Vector3(0,-5,0);
        startPos = endPos + new Vector3(0,20,0);
        lightning.GetComponent<LightningBoltScript>().StartPosition = startPos;
        lightning.GetComponent<LightningBoltScript>().EndPosition = endPos;

        if(PV.IsMine)
        {
            StartCoroutine(UseLightningCo());
        }
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
