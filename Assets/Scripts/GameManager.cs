using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] KeyCode keyCode;
    [SerializeField] GameObject pauseGamePanel;
    [SerializeField] Animator fadeImage;
    [SerializeField] float waitTimeBetweenLoad;
    [SerializeField] StarterAssetsInputs starterAssetsInputs;
    [SerializeField] LevelUpPanel levelUpPanel;

    [Header("Fireball")]
    [SerializeField] Image fireballBG;
    [SerializeField] TextMeshProUGUI fireballCDText;

    [Header("Lightning")]
    [SerializeField] Image lightningBG;
    [SerializeField] TextMeshProUGUI lightningCDText;

    [Header("Poison")]
    [SerializeField] Image poisonBG;
    [SerializeField] TextMeshProUGUI poisonCDText;

    [Header("Peoples")]
    [SerializeField] Villager[] villagers;
    [SerializeField] Transform monster;
    [SerializeField] Transform player;

    bool canUseAbility;
    bool goingHome;

    float playerSpeed;
    float playerSprintSpeed;
    float villagerSpeed;
    float monsterSpeed;

    public float GetPlayerSpeed
    {
        get
        {
            return playerSpeed;
        }
    }

    public float GetPLayerSprintSpeed
    {
        get
        {
            return playerSprintSpeed;
        }
    }

    public float GetVillagerSpeed
    {
        get
        {
            return villagerSpeed;
        }
    }

    public float GetMonsterSpeed
    {
        get
        {
            return monsterSpeed;
        }
    }

    public bool GetGoingHome
    {
        get
        {
            return goingHome;
        }
    }

    void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    
    }

    void Start() 
    {
        starterAssetsInputs.SetCursorState(true);
        canUseAbility = true;
        goingHome = false;
        if(!fadeImage.gameObject.activeSelf)
            fadeImage.gameObject.SetActive(true);
        SetUnFade();
        if(pauseGamePanel.activeSelf)
            pauseGamePanel.SetActive(false);

        //monsterSpeed = GetMonster.GetComponent<Monster>().navMeshAgent.speed;
        //villagerSpeed = villagers[0].navMeshAgent.speed;
        playerSpeed = GetPlayer.GetComponent<FirstPersonController>().MoveSpeed;
        playerSprintSpeed = GetPlayer.GetComponent<FirstPersonController>().SprintSpeed;
    }

/*
    void Update() 
    {
        if(Input.GetKeyDown(keyCode))
        {
            if(levelUpPanel.gameObject.activeSelf) { return; }
            if(!pauseGamePanel.activeSelf)
            {
                PauseGameEvent();
            }
            else
            {
                ContinueGameButtonEvent();
                starterAssetsInputs.SetCursorState(true);
            }
        }    
    }*/

    public LevelUpPanel GetLevelUpPanel
    {
        get
        {
            return levelUpPanel;
        }
    }

#region CanUseAbility
    public bool GetCanUseAbility
    {
        get
        {
            return canUseAbility;
        }
    }
    public void SetFalseCanUseAbility()
    {
        canUseAbility = false;
    }
    public void SetTrueCanUseAbility()
    {
        canUseAbility = true;
    }
#endregion

    public Transform GetNewTarget(Transform target)
    {/*
        foreach (Villager villager in villagers)
        {
            if(villager.GetIsInfected) 
            {
                continue;
            }
            else
            {
                float a = Mathf.Abs(Vector3.Distance(villager.transform.position, monster.position));
                float b = Mathf.Abs(Vector3.Distance(target.position, monster.position));
                if(target.GetComponent<Villager>() != null && target.GetComponent<Villager>().GetIsInfected || target == player) { b = Mathf.Infinity; }
                if(a < b) 
                {
                    target = villager.transform;
                }
            }
        }*/
        return target;
    }

    public Transform GetPlayer
    {
        get
        {
            return player;
        }
    }

    public Transform GetMonster
    {
        get
        {
            return monster;
        }
    }

#region FadeImageFunc
    void SetFade()
    {
        fadeImage.SetTrigger("Fade");
    }
    void SetUnFade()
    {
        fadeImage.SetTrigger("UnFade");
    }
#endregion

#region WinLose
    public void WinGame()
    {
        StartCoroutine(WinGameRoutine());
    }
    IEnumerator WinGameRoutine()
    {
        SetFade();
        yield return new WaitForSeconds(waitTimeBetweenLoad);
        SceneManager.LoadScene("Win");
    }
    public void LoseGame()
    {
        StartCoroutine(LoseGameRoutine());
    }
    IEnumerator LoseGameRoutine()
    {
        SetFade();
        yield return new WaitForSeconds(waitTimeBetweenLoad);
        SceneManager.LoadScene("Lose");
    }
#endregion

/*
#region GamePause
    public void PauseGameEvent()
    {
        Time.timeScale = 0;
        levelUpPanel.IsThinking = true;
        pauseGamePanel.SetActive(true);
        starterAssetsInputs.SetCursorState(false);
    }
    public void ContinueGameButtonEvent()
    {
        Time.timeScale = 1;
        levelUpPanel.IsThinking = false;
        pauseGamePanel.SetActive(false);
        starterAssetsInputs.SetCursorState(true);
    }
    public void HomeButtonEvent()
    {
        StartCoroutine(HomeButtonRoutine());
        goingHome = true;
        Time.timeScale = 1;
    }
    IEnumerator HomeButtonRoutine()
    {
        SetFade();
        yield return new WaitForSeconds(waitTimeBetweenLoad);
        SceneManager.LoadScene("MainMenu");
    }
    public void QuitButtonEvent()
    {
        StartCoroutine(QuitButtonRoutine());
        goingHome = true;
        Time.timeScale = 1;
    }
    IEnumerator QuitButtonRoutine()
    {
        SetFade();
        yield return new WaitForSeconds(waitTimeBetweenLoad);
        Application.Quit();
    }
#endregion
*/

    public void StopAllSFX()
    {
        foreach (var item in villagers)
        {/*
            if(item.GetIsDead) { continue; }
            item.StopRunSFX();
            item.StopWalkSFX();*/
        }
        //monster.GetComponent<Monster>().StopWalkSFX();
    }

}
