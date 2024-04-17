using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePotions : PotionsAbstract
{
    public static IcePotions Instance;

    void Awake() 
    {
        Instance = this;
    }

}
