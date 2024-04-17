using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    [SerializeField] GameObject visualObj;

    void Awake() 
    {
        visualObj.SetActive(false);
    }

}
