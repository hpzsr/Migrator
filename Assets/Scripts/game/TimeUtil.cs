using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtil : MonoBehaviour
{
    float scale = 0.5f;
    void Start()
    {
        InvokeRepeating("onInvoke", 1.0f * scale, 1.0f * scale);
    }
    
    void onInvoke()
    {
        FamilyLayer.s_instance.addTime(1.0f / 60.0f * scale);
    }
}
