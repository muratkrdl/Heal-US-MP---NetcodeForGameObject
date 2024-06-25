using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DoctorMelee : NetworkBehaviour
{
    [SerializeField] Image meleeBar;
    [SerializeField] Image meleeBarBG;

    [SerializeField] Camera cam;

    [SerializeField] LayerMask layerMask;
    [SerializeField] float range;
    [SerializeField] float damage;

    [SerializeField] float lerpTime;

    float initialLerpTime;

    float currentAmount = 0;

    bool canMeleeAttack = true;

    bool combo;

    public bool CanMeleeAttack
    {
        get
        {
            return canMeleeAttack;
        }
        set
        {
            canMeleeAttack = value;
        }
    }

    void Start() 
    {
        SetMeleeBar(false);
        initialLerpTime = lerpTime;
    }

    public void MeleeAnimationEvent()
    {
        canMeleeAttack = false;
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f,.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray, out RaycastHit hit, range, layerMask))
        {
            if(hit.transform.TryGetComponent<Villager>(out var villager))
            {
                villager.GethHealDamageServerRpc(true, damage / 2);
                Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
                SoundManager.Instance.PlaySound3D("Punch", transform.position); // herkese çal
            }
            else if(hit.transform.TryGetComponent<Patrick>(out var patrick))
            {
                patrick.GetDamage(damage);
                Manager.Instance.SpawnFloatingText(transform.position, damage.ToString(), Color.red);
                SoundManager.Instance.PlaySound3D("Punch", transform.position); // herkese çal
            }
        }
        
        StartCoroutine(MeleeBarCo());
    }

    IEnumerator MeleeBarCo()
    {
        SetMeleeBar(true);
        lerpTime += .5f;
        float elapsed = 0;
        float t = 0;
        while(true)
        {
            elapsed += Time.deltaTime;
            t = elapsed / lerpTime;
            yield return null;
            currentAmount = Mathf.Lerp(currentAmount, 1, Time.deltaTime * t);

            if(currentAmount >= .96f)
            {
                currentAmount = 1;
                SetMeleeBar(false);
                currentAmount = 0;
                if(!combo) 
                { 
                    combo = true; 
                    Invoke(nameof(SetInitialValueMeleeCounter),9);
                }
                canMeleeAttack = true;
                break;
            }
            UpdateCurrentValues();
        }
        StopCoroutine(MeleeBarCo());
    }

    void SetInitialValueMeleeCounter()
    {
        lerpTime = initialLerpTime;
        combo = false;
    }

    void UpdateCurrentValues()
    {
        meleeBar.fillAmount = currentAmount;
    }

    void SetMeleeBar(bool value)
    {
        meleeBarBG.enabled = value;
        meleeBar.enabled = value;
    }

}
