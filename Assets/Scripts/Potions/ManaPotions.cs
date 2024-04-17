using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotions : PotionsAbstract
{
    public static ManaPotions Instance;

    void Awake() 
    {
        Instance = this;
    }
    
}
