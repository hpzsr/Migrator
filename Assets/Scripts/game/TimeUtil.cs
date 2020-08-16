using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtil : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("onInvoke", 1, 1);
    }
    
    void onInvoke()
    {
        FamilyLayer.s_instance.addTime(1.0f / 60.0f);
    }
}
