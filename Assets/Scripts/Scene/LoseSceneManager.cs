using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneManager : MonoBehaviour
{
    [SerializeField] Animator[] villagers;

    [SerializeField] Animator fadeImage;

    void Awake() 
    {
        if(!fadeImage.gameObject.activeSelf)
            fadeImage.gameObject.SetActive(true);
        SetUnFade();
    }

    void Start() 
    {
        SetDieAllVillagers();
        Cursor.lockState = CursorLockMode.None;

        LobbyManager.Instance.KYS();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
    }

    void SetDieAllVillagers()
    {
        foreach (Animator villager in villagers)
        {
            if(Random.Range(0,2) == 0)
            {
                villager.SetTrigger("Die1");
            }
            else
            {
                villager.SetTrigger("Die2");
            }
        }
    }

    public void HomeButtonEvent()
    {
        SetFade();
        Invoke(nameof(HomeButton),1);
    }

    void HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void SetFade()
    {
        fadeImage.SetTrigger("Fade");
    }

    void SetUnFade()
    {
        fadeImage.SetTrigger("UnFade");
    }

}
