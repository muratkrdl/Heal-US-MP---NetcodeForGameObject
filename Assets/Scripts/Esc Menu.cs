using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject escMenu;

    [SerializeField] FirstPersonController firstPersonController;
    [SerializeField] FreeCameraMovement freeCameraMovement;

    [SerializeField] KeyCode escMenuKeyCode;
    
    [SerializeField] Animator fadeImage;

    [SerializeField] Image masterSwitched;

    bool isThinking = false;
    bool masterSwitchedBool = false;

    public bool GetIsThinking
    {
        get
        {
            return isThinking;
        }
    }

    void Start() 
    {
        SetUnFade();
    }

    void Update() 
    {
        if(masterSwitchedBool) { return; }

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        masterSwitchedBool = true;
        isThinking = true;
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

        masterSwitched.gameObject.SetActive(true);
        Invoke(nameof(MasterClientSwitched), 1.5f);
    }

    void MasterClientSwitched()
    {
    }

    void HomeButtonEvent()
    {
        Destroy(RoomManager.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnClick_Quit()
    {
        SetFade();
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
