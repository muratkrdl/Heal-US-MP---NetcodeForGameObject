using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    [SerializeField] ParticleSystem poisonVFX;

    public void OpenPoison()
    {
        poisonVFX.Play();
    }

    public void ClosePoison()
    {
        poisonVFX.Stop();
    }

}
