using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneManager : MonoBehaviourPunCallbacks
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
        Destroy(RoomManager.Instance.gameObject);
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
