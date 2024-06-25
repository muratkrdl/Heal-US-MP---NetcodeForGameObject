using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] Transform[] patrickTransforms;
    [SerializeField] Transform[] doctorTransforms;
    [SerializeField] Transform[] villagerTransforms;

    public Transform[] GetPatrickTransforms
    {
        get
        {
            return patrickTransforms;
        }
    }
    public Transform[] GetDoctorTransforms
    {
        get
        {
            return doctorTransforms;
        }
    }
    public Transform[] GetVillagerTransforms
    {
        get
        {
            return villagerTransforms;
        }
    }

    void Awake() 
    {
        Instance = this;    
    }

}
