using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowFPS : MonoBehaviour
{
    Text text;
    float refreshDur = 0.5f;

    void Start()
    {
        GameObject obj = CommonUtil.createObjFromPrefab(transform,"Prefabs/UI/Text");
        obj.transform.position = new Vector3(Screen.width / 2, Screen.height - 20, 0);
        text = obj.GetComponent<Text>();
        text.fontSize = 22;

        InvokeRepeating("onInvoke", refreshDur, refreshDur);
    }

    void onInvoke()
    {
        text.text = "fps:" + ((int)(1.0f / Time.deltaTime)).ToString();
    }
}
