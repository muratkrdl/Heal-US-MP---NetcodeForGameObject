using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    
    [SerializeField] Animator fadeImage;

    [SerializeField] float waitTimeBetweenLoad;

    [SerializeField] Menu[] menus;

    void Awake() 
    {
        Instance = this;
    }

    void Start() 
    {
        Cursor.lockState = CursorLockMode.None;
        if(!fadeImage.gameObject.activeSelf)
        {
            fadeImage.gameObject.SetActive(true);
        }
        
        SetUnFade();
        OpenMenu("mainmenu");
    }

    public void OpenMenu(string menuName)
    {
        foreach (var item in menus)
        {
            if(item.menuName == menuName)
            {
                item.Open();
                continue;
            }
            else if(item.open)
            {
                CloseMenu(item);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (var item in menus)
        {
            if(item.open)
            {
                CloseMenu(item);
            }
        }
        menu.Open();
    }

    void CloseMenu(Menu menu)
    {
        menu.Close();
    }

#region ButtonEvents
    public void SinglePlayerButtonEvent()
    {
        SetFade();
        Invoke(nameof(StartGame),waitTimeBetweenLoad);
    }
    public void QuitButtonEvent()
    {
        SetFade();
        Invoke(nameof(QuitGame),waitTimeBetweenLoad);
    }
#endregion

#region FadeImageFunc
    public void SetFade()
    {
        fadeImage.SetTrigger("Fade");
    }
    public void SetUnFade()
    {
        fadeImage.SetTrigger("UnFade");
    }
#endregion

    void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    void QuitGame()
    {
        Application.Quit();
    }

}
