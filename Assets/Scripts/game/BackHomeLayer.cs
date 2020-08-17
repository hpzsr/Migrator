using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHomeLayer : LayerBase
{
    void Start()
    {
        TimerUtil.getInstance().delayTime(3, () =>
        {
            LayerManager.showLayer(Consts.Layer.ReadyNightLayer);
            Destroy(gameObject);
        });
    }

    void Update()
    {

    }
}
