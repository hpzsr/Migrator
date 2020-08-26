using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager
{
    public static GameObject showLayer(Consts.Layer layerName, Event.EventCallBackData data = null)
    {
        GameObject layer = null;
        if (layerName == Consts.Layer.FamilyLayer)
        {
            layer = CommonUtil.createObjFromPrefab(GameObject.Find("Canvas").transform, "Prefabs/Layers/" + layerName.ToString());
        }
        else
        {
            layer = CommonUtil.createObjFromPrefab(GameObject.Find("Canvas_High").transform, "Prefabs/Layers/" + layerName.ToString());
        }
        layer.GetComponent<LayerBase>().startData = data;

        return layer;
    }

    public static void closeLayer(GameObject layer)
    {
        GameObject.Destroy(layer);
    }
}
