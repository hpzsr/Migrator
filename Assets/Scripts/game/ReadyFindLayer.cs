using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyFindLayer : LayerBase
{
    public Transform left;
    public Transform right;

    int choiceMap = 1;

    public static ReadyFindLayer s_instance = null;

    void Start()
    {
        s_instance = this;

        for (int i = 0; i < right.childCount; i++)
        {
            Button btn = right.GetChild(i).GetComponent<Button>();
            btn.onClick.AddListener(()=>
            {
                for(int j = 0; j < right.childCount; j++)
                {
                    right.GetChild(j).GetComponent<Image>().color = new Color(1,1,1,1);
                }
                btn.GetComponent<Image>().color = new Color(0, 1, 0, 1);
                choiceMap = int.Parse(btn.transform.name.Split('_')[2]);
            });

            if(i == 0)
            {
                btn.GetComponent<Image>().color = new Color(0, 1, 0, 1);
            }
        }
    }

    public void onClickEnter()
    {
        for (int i = 0; i < left.childCount; i++)
        {
            if (left.GetChild(i).Find("Toggle_1").GetComponent<Toggle>().isOn)
            {
                GameData.getInstance().curGetRewardList.Clear();
                Event.EventCallBackData data = new Event.EventCallBackData();
                data.data_int = choiceMap;
                data.data_int = 2;
                data.data_string = (100 + i + 1) + "";
                LayerManager.showLayer(Consts.Layer.MapLayer, data);

                onClickBack();

                return;
            }
        }

        ToastScript.show("Must have one character to explore");
    }
    public void onClickBack()
    {
        s_instance = null;
        Destroy(gameObject);
    }
}
