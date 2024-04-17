using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPPotions : PotionsAbstract
{
    public static HPPotions Instance;

    void Awake() 
    {
        Instance = this;
    }
    
}
