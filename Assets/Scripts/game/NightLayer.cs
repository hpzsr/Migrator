﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightLayer : LayerBase
{
    void Start()
    {
        TimerUtil.getInstance().delayTime(3, () =>
         {
             FamilyLayer.s_instance.newDay();
             Destroy(gameObject);
         });
    }
    
    void Update()
    {
        
    }
}
