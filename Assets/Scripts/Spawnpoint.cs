using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] GameObject visual;

    void Start()
    {
        visual.SetActive(false);    
    }

}
