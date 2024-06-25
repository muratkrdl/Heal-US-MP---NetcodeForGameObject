using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenu : MonoBehaviour
{
    [SerializeField] GameObject escMenu;

    [SerializeField] FreeCameraMovement freeCameraMovement;

    [SerializeField] KeyCode escMenuKeyCode;
    
    [SerializeField] Animator fadeImage;

    [SerializeField] GameObject requiredTypeDisconnectedPanel;

    FirstPersonController firstPersonController;
    
    bool isThinking = false;

    public FirstPersonController FirstPersonController
    {
        get
        {
            return firstPersonController;
        }
        set
        {
            firstPersonController = value;
        }
    }

    public bool GetIsThinking
    {
        get
        {
            return isThinking;
        }
    }

    public void SetRequiredTypeDisconnectedPanel(bool value)
    {
        requiredTypeDisconnectedPanel.SetActive(value);
        escMenu.SetActive(!value);
    }

    void Start() 
    {
        fadeImage.SetTrigger("UnFade");
    }

    void Update() 
    {
        if(Manager.Instance.RequiredTypeHasDisconnected) { return; }

        if(Input.GetKeyDown(escMenuKeyCode))
        {
            isThinking = !isThinking;
            if(isThinking)
            {
                if(firstPersonController != null)
                {
                    firstPersonController.ResetAllInputs();
                    firstPersonController.SetCursorState(!isThinking);
                }
                else
                {
                    freeCameraMovement.ResetAllInputs();
                    freeCameraMovement.SetCursorState(!isThinking);
                }
            }
            else
            {
                if(firstPersonController != null)
                {
                    firstPersonController.SetCursorState(!isThinking);
                }
                else
                {
                    freeCameraMovement.SetCursorState(!isThinking);
                }
            }
            
            escMenu.SetActive(isThinking);
        }
    }

    public void OnClick_Continue()
    {
        isThinking = false;
        if(firstPersonController == null)
        {
            freeCameraMovement.SetCursorState(!isThinking);
        }
        else
        {
            firstPersonController.SetCursorState(!isThinking);
        }
        escMenu.SetActive(isThinking);
    }

    public void OnClick_Home()
    {
        SetFade();
        Invoke(nameof(HomeButtonEvent), 1.5f);
    }

    void HomeButtonEvent()
    {
        if(!Manager.Instance.RequiredTypeHasDisconnected)
        {
            var type = firstPersonController.HumanType;
            if(type == HumanType.patrick || type == HumanType.doctor)
            {
                Manager.Instance.RequiredTypeHasDisconnectedServerRpc();
            }
        }
        LobbyManager.Instance.KYS();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu"); 
    }

    public void OnClick_Quit()
    {
        SetFade();
        Manager.Instance.Application_Quitting();
        Invoke(nameof(QuitGame), 1.5f);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    public void SetFade()
    {
        fadeImage.SetTrigger("Fade");
    }
    public void SetUnFade()
    {
        fadeImage.SetTrigger("UnFade");
    }

}
