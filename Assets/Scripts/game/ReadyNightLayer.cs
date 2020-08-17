using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyNightLayer : LayerBase
{
    public Transform left;
    public Transform right;

    List<int> playerJob_list = new List<int>() { 0, 0, 0, 0 };

    public static ReadyNightLayer s_instance = null;

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
    }

    void setPlayerJob(int playerIndex, int jobType)
    {
        playerJob_list[playerIndex - 1] = jobType;

        // 人员分配数量
        {
            for (int i = 1; i <= 2; i++)
            {
                int count = 0;
                for (int j = 0; j < playerJob_list.Count; j++)
                {
                    if (playerJob_list[j] == i)
                    {
                        ++count;
                    }
                }

                for (int j = 0; j < left.childCount; j++)
                {
                    Text text = left.GetChild(j).Find("Toggle_" + i + "/playerCount").GetComponent<Text>();
                    
                    text.text = "(" + count + "/4)";
                    if (count >= 4)
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

    public void onClickEnter()
    {
        LayerManager.showLayer(Consts.Layer.NightLayer);
        close();
    }
    public void close()
    {
        s_instance = null;
        Destroy(gameObject);
    }
}
