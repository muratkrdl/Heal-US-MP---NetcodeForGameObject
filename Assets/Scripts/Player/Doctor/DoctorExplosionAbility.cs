using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoctorExplosionAbility : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highExplosionCDText;
    [SerializeField] Image highExplosionBG;

    [SerializeField] PhotonView PV;

    [SerializeField] GameObject energyExplosionPrefab;
    [SerializeField] Camera cam;

    [SerializeField] int abilityCD;

    [SerializeField] float damage;
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
        Mana.Instance.DecreaseMana(manaCost);
        
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hitData,Mathf.Infinity))
        {
            PV.RPC(nameof(HighExplosion),RpcTarget.All,hitData.point);
        }
    }

    [PunRPC] void HighExplosion(Vector3 pos)
    {
        var highExplosion = Instantiate(energyExplosionPrefab,pos,Quaternion.identity);
        highExplosion.GetComponent<EnergyExplosion>().SetDamage = damage;

        if(PV.IsMine)
        {
            StartCoroutine(UseHighExplosionCo());
        }
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
