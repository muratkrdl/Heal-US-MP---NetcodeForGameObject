using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviour
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
        SetWinAllVillagers();
        Cursor.lockState = CursorLockMode.None;

        LobbyManager.Instance.KYS();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
    }

    void SetWinAllVillagers()
    {
        foreach (Animator villager in villagers)
        {
            villager.SetTrigger("Win");
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
