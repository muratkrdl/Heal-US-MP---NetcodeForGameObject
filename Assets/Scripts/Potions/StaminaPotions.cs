using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPotions : PotionsAbstract
{
    public static StaminaPotions Instance;

    void Awake() 
    {
        Instance = this;
    }
    
}
