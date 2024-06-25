using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayMenu : MonoBehaviour
{
    [SerializeField] GameObject[] humanTypesPanels;

    int choosenIndex = 0;

    void OpenChoosenPanel(int index)
    {
        foreach (var item in humanTypesPanels)
        {
            item.SetActive(false);
        }

        humanTypesPanels[index].SetActive(true);
    }

    public void OnClick_DecreaseChoosenIndex()
    {
        choosenIndex--;
        if(choosenIndex < 0)
            choosenIndex = humanTypesPanels.Length -1;

        OpenChoosenPanel(choosenIndex);
    }

    public void OnClick_IncreaseChoosenIndex()
    {
        choosenIndex++;
        if(choosenIndex > humanTypesPanels.Length -1)
            choosenIndex = 0;
        
        OpenChoosenPanel(choosenIndex);
    }

}
