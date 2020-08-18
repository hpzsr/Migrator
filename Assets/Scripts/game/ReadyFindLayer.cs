using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyFindLayer : LayerBase
{
    public Transform left;
    public Transform right;

    List<int> playerJob_list = new List<int>() { 0, 0, 0, 0};

    int choiceMap = 1;

    public static ReadyFindLayer s_instance = null;

    void Start()
    {
        s_instance = this;

        for (int i = 0; i < left.childCount; i++)
        {
            int playerIndex = i + 1;
            for (int j = 0; j < left.GetChild(i).childCount; j++)
            {
                int toggleIndex = j;
                Transform trans = left.GetChild(i);
                Toggle toggle = trans.GetChild(j).GetComponent<Toggle>();
                if (toggle)
                {
                    toggle.onValueChanged.AddListener((bool b) =>
                    {
                        if (b)
                        {
                            setPlayerJob(playerIndex, toggleIndex);
                        }
                        else
                        {
                            setPlayerJob(playerIndex, 0);
                        }
                    });
                }
            }
        }

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
    
    void setPlayerJob(int playerIndex,int jobType)
    {
        playerJob_list[playerIndex - 1] = jobType;
        
        // 探索
        if (jobType == 1)
        {
            for (int i = 0; i < playerJob_list.Count; i++)
            {
                if ((i != (playerIndex - 1)) && (playerJob_list[i] == jobType))
                {
                    playerJob_list[i] = 0;
                    left.Find("player_" + (i + 1) + "/Toggle_" + jobType).GetComponent<Toggle>().isOn = false;
                }
            }
        }
        // 保护 建造 睡觉
        else if (jobType != 0)
        {
            int repeatCount = 1;
            for (int i = playerJob_list.Count - 1; i >= 0; i--)
            {
                if ((i != (playerIndex - 1)) && (playerJob_list[i] == jobType))
                {
                    if(++repeatCount > 2)
                    {
                        playerJob_list[i] = 0;
                        left.Find("player_" + (i + 1) + "/Toggle_" + jobType).GetComponent<Toggle>().isOn = false;
                    }
                }
            }
        }

        // 人员分配数量
        {
            for(int i = 1; i <= 4; i++)
            {
                int count = 0;
                for(int j = 0; j < playerJob_list.Count; j++)
                {
                    if(playerJob_list[j] == i)
                    {
                        ++count;
                    }
                }

                for (int j = 0; j < left.childCount; j++)
                {
                    Text text = left.GetChild(j).Find("Toggle_" + i + "/playerCount").GetComponent<Text>();
                    if (i == 1)
                    {
                        text.text = "(" + count + "/1)";
                        if(count >= 1)
                        {
                            text.color = new Color(1, 0, 0);
                        }
                        else
                        {
                            text.color = new Color(1, 1, 1);
                        }
                    }
                    else
                    {
                        text.text = "(" + count + "/2)";
                        if (count >= 2)
                        {
                            text.color = new Color(1, 0, 0);
                        }
                        else
                        {
                            text.color = new Color(1, 1, 1);
                        }
                    }
                }
            }
        }
    }

    public void onClickEnter()
    {
        for(int i = 0; i < playerJob_list.Count; i++)
        {
            if(playerJob_list[i] == 1)
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
