﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class ToastScript : MonoBehaviour {

    static Transform s_canvas = null;

    public static void show (string text)
    {
        if(s_canvas == null)
        {
            s_canvas = GameObject.Find("Canvas_High").transform;
        }

        GameObject prefab = Resources.Load("Prefabs/Commons/Toast") as GameObject;
        GameObject obj = MonoBehaviour.Instantiate(prefab, s_canvas);
        obj.transform.Find("Text").GetComponent<Text>().text = text;
        // 立即刷新Text尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(obj.transform.Find("Text").GetComponent<RectTransform>());
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(obj.transform.Find("Text").GetComponent<RectTransform>().sizeDelta.x + 50, 80);

        obj.transform.DOLocalMove(new Vector3(0, 200, 0), 1).OnComplete(() => {
            Destroy(obj);
        });
    }
}
