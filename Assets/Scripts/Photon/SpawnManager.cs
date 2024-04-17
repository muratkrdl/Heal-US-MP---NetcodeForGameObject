using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] Spawnpoint[] patrickSpawnPoints;
    [SerializeField] Spawnpoint[] doctorSpawnPoints;
    [SerializeField] List<Spawnpoint> villagerSpawnPoints;

    void Awake() 
    {
        Instance = this;    
    }

    public Transform GetPatrickSpawnpoint
    {
        get
        {
            return patrickSpawnPoints[Random.Range(0,patrickSpawnPoints.Length)].transform;
        }
    }

    public Transform GetDoctorSpawnpoint
    {
        get
        {
            return doctorSpawnPoints[Random.Range(0,doctorSpawnPoints.Length)].transform;
        }
    }

    public Transform GetVillagerSpawnPoint()
    {
        int index = Random.Range(0,villagerSpawnPoints.Count);
        Transform spawnPoint = villagerSpawnPoints[index].transform;
        villagerSpawnPoints.Remove(villagerSpawnPoints[index]);
        return spawnPoint;
    }

}
