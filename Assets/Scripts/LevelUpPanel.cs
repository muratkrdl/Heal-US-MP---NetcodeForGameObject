using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    /*
    [SerializeField] StarterAssetsInputs starterAssetsInputs;

    [SerializeField] IndexPanel indexPanel;

    [SerializeField] GameObject[] levelUpGameobjectsPart1;
    [SerializeField] GameObject[] levelUpGameobjectsPart2;

    [Header("Fireball")]
    [SerializeField] AbilityFireball abilityFireball;
    [SerializeField] TextMeshProUGUI currentFireballStunTime;
    [SerializeField] TextMeshProUGUI levelUpFireballStunTime;

    [SerializeField] int increaseAmountFireballStunTime;


    [Header("Lightning")]
    [SerializeField] AbilityLightning abilityLightning;
    [SerializeField] TextMeshProUGUI currentLightningDamage;
    [SerializeField] TextMeshProUGUI levelUpLightningDamage;

    [SerializeField] int increaseAmountLightningDamage;


    [Header("Poison")]
    [SerializeField] MonsterCollisionParticle abilityPoison;
    [SerializeField] TextMeshProUGUI currentPoisonDamage;
    [SerializeField] TextMeshProUGUI levelUpPoisonDamage;

    [SerializeField] int increaseAmountPoisonDamage;


    [Header("HP")]
    [SerializeField] PlayerHP playerHP;
    [SerializeField] TextMeshProUGUI currentMaxHP;
    [SerializeField] TextMeshProUGUI levelUpMaxHP;

    [SerializeField] int increaseAmountMaxHP;

    
    [Header("Mana")]
    [SerializeField] Mana playerMana;
    [SerializeField] TextMeshProUGUI currentMaxMana;
    [SerializeField] TextMeshProUGUI levelUpMaxMana;

    [SerializeField] int increaseAmountMaxMana;


    [Header("Stamina")]
    [SerializeField] Stamina playerStamina;
    [SerializeField] TextMeshProUGUI currentMaxStamina;
    [SerializeField] TextMeshProUGUI levelUpMaxStamina;

    [SerializeField] int increaseAmountMaxStamina;


    [Header("Ice")]
    [SerializeField] UseIcePotion icePotion;
    [SerializeField] TextMeshProUGUI currentSlowAmount;
    [SerializeField] TextMeshProUGUI levelUpSlowAmount;

    [SerializeField] int increaseAmountSlowAmount;

    bool isThinking;

    public bool IsThinking
    {
        get
        {
            return isThinking;
        }
        set
        {
            isThinking = value;
        }
    }

    void Start() 
    {
        UpdateAllTextValues();
    }

    void OnEnable()
    {
        GameManager.Instance.StopAllSFX();
        isThinking = true;
        UpdateAllTextValues();
        CallLevelUpPanel();
    }

    void CallLevelUpPanel()
    {
        foreach (var item in levelUpGameobjectsPart1)
        {
            item.SetActive(false);
        }
        foreach (var item in levelUpGameobjectsPart2)
        {
            item.SetActive(false);
        }
        levelUpGameobjectsPart1[Random.Range(0,levelUpGameobjectsPart1.Length)].SetActive(true);
        levelUpGameobjectsPart2[Random.Range(0,levelUpGameobjectsPart2.Length)].SetActive(true);
    }

    void UpdateAllTextValues()
    {
        currentFireballStunTime.text = abilityFireball.StunTime.ToString();
        levelUpFireballStunTime.text = (abilityFireball.StunTime + increaseAmountFireballStunTime).ToString();

        currentLightningDamage.text = abilityLightning.Damage.ToString();
        levelUpLightningDamage.text = (abilityLightning.Damage + increaseAmountLightningDamage).ToString();

        currentPoisonDamage.text = abilityPoison.PoisonDamage.ToString();
        levelUpPoisonDamage.text = (abilityPoison.PoisonDamage + increaseAmountPoisonDamage).ToString();

        currentMaxHP.text = playerHP.MaxHP.ToString();
        levelUpMaxHP.text = (playerHP.MaxHP + increaseAmountMaxHP).ToString();

        currentMaxMana.text = playerMana.MaxMana.ToString();
        levelUpMaxMana.text = (playerMana.MaxMana + increaseAmountMaxMana).ToString();

        currentMaxStamina.text = playerStamina.MaxStamina.ToString();
        levelUpMaxStamina.text = (playerStamina.MaxStamina + increaseAmountMaxStamina).ToString();

        currentSlowAmount.text = icePotion.SlowAmount.ToString();
        levelUpSlowAmount.text = (icePotion.SlowAmount + increaseAmountSlowAmount).ToString();
    }

#region ButtonEvents
    public void LevelUPFireball()
    {
        abilityFireball.StunTime = abilityFireball.StunTime + increaseAmountFireballStunTime;
        PushButton();
    }
    public void LevelUPLightning()
    {
        abilityLightning.Damage = abilityLightning.Damage + increaseAmountLightningDamage;
        PushButton();
    }
    public void LevelUPPoison()
    {
        abilityPoison.PoisonDamage = abilityPoison.PoisonDamage + increaseAmountSlowAmount;
        PushButton();
    }
    public void LevelUPHP()
    {
        playerHP.MaxHP = playerHP.MaxHP + increaseAmountMaxHP;
        playerHP.UpdateValue();
        PushButton();
    }
    public void LevelUPMana()
    {
        playerMana.MaxMana = playerMana.MaxMana + increaseAmountMaxMana;
        playerMana.UpdateValue();
        PushButton();
    }
    public void LevelUPStamina()
    {
        playerStamina.MaxStamina = playerStamina.MaxStamina + increaseAmountMaxStamina;
        playerStamina.UpdateValue();
        playerStamina.LevelUpMaxStamina();
        PushButton();
    }
    public void LevelUPIce()
    {
        icePotion.SlowAmount = icePotion.SlowAmount + increaseAmountSlowAmount;
        PushButton();
    }
#endregion

    void PushButton()
    {
        isThinking = false;
        gameObject.SetActive(false);
    }
*/
}
