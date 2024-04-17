using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskImageItem : MonoBehaviour
{
    [SerializeField] Image taskImage;
    [SerializeField] TextMeshProUGUI taskText;

    public Image TaskImage
    {
        get
        {
            return taskImage;
        }
        set
        {
            taskImage = value;
        }
    }

    public TextMeshProUGUI TaskText
    {
        get
        {
            return taskText;
        }
        set
        {
            taskText = value;
        }
    }

}
