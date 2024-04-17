using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField] string mainMenuName;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
    }
}
