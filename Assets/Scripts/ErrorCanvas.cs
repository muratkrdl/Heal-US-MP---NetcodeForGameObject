using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ErrorCanvas : MonoBehaviour
{
    [SerializeField] MainMenuManager obj;

    [SerializeField] Menu menu;

    [SerializeField] Canvas mainMenuCanvas;

    void Start() 
    {
        StartCoroutine(CheckCloseObj());
    }

    IEnumerator CheckCloseObj()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            if(obj == null)
            {
                menu.Open();
                break;
            }
        }

        StopCoroutine(CheckCloseObj());
    }

    public void OpenGameObj()
    {
        menu.Close();
        var newCanvas = Instantiate(mainMenuCanvas);
        obj = newCanvas.GetComponent<MainMenuManager>();

        if(PhotonNetwork.IsConnected)
            newCanvas.GetComponent<MainMenuManager>().OpenMenu("title");
        else
            newCanvas.GetComponent<MainMenuManager>().OpenMenu("mainmenu");
        obj.gameObject.SetActive(true);
        StartCoroutine(CheckCloseObj());
    }

}
