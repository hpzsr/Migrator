using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FamilyLayer : LayerBase
{
    public static FamilyLayer s_instance = null;
    
    public Canvas canvas;
    public Image floor1;
    public Image floor2;
    public Image floor3;
    public Transform playerInfo;
    public Transform parts;

    public Text text_time;
    int day = 1;
    float curTime = 6;
    bool isBeyond12 = false;
    bool isBeyond19 = false;

    public List<PlayerScript> list_player = new List<PlayerScript>();
    public PlayerScript curControlPlayer;

    public List<Transform> list_bed = new List<Transform>();
    public List<Transform> list_ladder = new List<Transform>();
    public List<ItemScript> list_item = new List<ItemScript>();

    public Transform trans_workbench;
    public Transform trans_toy;
    public Transform trans_vase;
    public Transform trans_gitar;
    public Transform trans_kitchen;

    void Start()
    {
        s_instance = this;

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        for (int i = 0; i < parts.childCount; i++)
        {
            Transform trans = parts.GetChild(i);

            if(trans.GetComponent<ItemScript>() != null)
            {
                trans.GetComponent<ItemScript>().init();
                list_item.Add(trans.GetComponent<ItemScript>());
            }

            if (trans.name == "alphaSpace")
            {
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    float pianyi = 250;
                    bool isMove = false;
                    Vector2 vec2 = getMousePosition();
                    if ((vec2.y >= floor1.transform.localPosition.y) && (vec2.y - floor1.transform.localPosition.y) <= pianyi)
                    {
                        isMove = true;
                    }
                    else if ((vec2.y >= floor2.transform.localPosition.y) && (vec2.y - floor2.transform.localPosition.y) <= pianyi)
                    {
                        isMove = true;
                    }
                    else if ((vec2.y >= floor3.transform.localPosition.y) && (vec2.y - floor3.transform.localPosition.y) <= pianyi)
                    {
                        isMove = true;
                    }

                    if (isMove)
                    {
                        curControlPlayer.moveTo(Consts.MoveEndEvent.Nothing, vec2);
                    }
                });
            }
            else if (trans.tag == "Player")
            {
                PlayerScript playerScript = trans.GetComponent<PlayerScript>();
                playerScript.init();
                list_player.Add(playerScript);
                trans.Find("btn_choice").GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer = playerScript;

                    for (int j = 0; j < list_player.Count; j++)
                    {
                        if (list_player[j] == curControlPlayer)
                        {
                            list_player[j].setIsChoice(true);
                        }
                        else
                        {
                            list_player[j].setIsChoice(false);
                        }
                    }
                });
            }
            else if (trans.tag == "bed")
            {
                list_bed.Add(trans);
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Bed, trans.transform.localPosition, trans);
                });
            }
            else if (trans.tag == "ladder")
            {
                list_ladder.Add(trans);
            }
            else if (trans.tag == "workbench")
            {
                trans_workbench = trans;
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Make, trans.transform.localPosition, trans);
                });
            }
            else if (trans.tag == "toy")
            {
                trans_toy = trans;
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Toy, trans.transform.localPosition, trans_workbench);
                });
            }
            else if (trans.tag == "vase")
            {
                trans_vase = trans;
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Vase, trans.transform.localPosition, trans_workbench);
                });
            }
            else if (trans.tag == "gitar")
            {
                trans_gitar = trans;
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Gitar, trans.transform.localPosition, trans_workbench);
                });
            }
            else if (trans.tag == "heater")
            {
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Nothing, trans.transform.localPosition, trans);
                });
            }
            else if (trans.tag == "kitchen")
            {
                trans_kitchen = trans;
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Make, trans.transform.localPosition, trans);
                });
            }
            else if (trans.tag == "find")
            {
                trans.GetComponent<Button>().onClick.AddListener(() =>
                {
                    curControlPlayer.moveTo(Consts.MoveEndEvent.Find, trans.transform.localPosition, trans);
                });
            }
        }

        setCurTime();
        Invoke("onInvoke", 0.1f);
    }

    void Update()
    {
        // 增加背包数据
        if(Input.GetKeyDown(KeyCode.A))
        {
            for (int i = 201; i <= 208; i++)
            {
                GameData.getInstance().changeBagItemCount(i,1000);
            }

            ToastScript.show("Add materials success");
        }
    }

    private void onInvoke()
    {
        if (list_player.Count > 0)
        {
            curControlPlayer = list_player[0];
            curControlPlayer.setIsChoice(true);
        }
    }

    public PlayerScript getPlayerScriptById(int id)
    {
        for(int i = 0; i < list_player.Count; i++)
        {
            if(list_player[i].playerID == id)
            {
                return list_player[i];
            }
        }

        return null;
    }

    public float getFloorPosY(int floor)
    {
        if (floor == 1)
        {
            return FamilyLayer.s_instance.floor1.transform.localPosition.y;
        }
        else if (floor == 2)
        {
            return FamilyLayer.s_instance.floor2.transform.localPosition.y;
        }
        else if (floor == 3)
        {
            return FamilyLayer.s_instance.floor3.transform.localPosition.y;
        }

        return 0;
    }
    
    // 鼠标坐标转UI坐标
    public Vector2 getMousePosition()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                    Input.mousePosition, canvas.worldCamera, out pos);
        return pos;
    }

    public void setColorByLevel(Image image,int level)
    {
        switch (level)
        {
            case 0:
                {
                    image.color = new Color(100.0f / 255.0f, 100.0f / 255.0f, 100.0f / 255.0f, 120.0f / 255.0f);
                    break;
                }
            default:
                {
                    image.color = new Color(1,1,1,1);
                    break;
                }
        }
    }

    public void onClickBag()
    {
        LayerManager.showLayer(Consts.Layer.BagLayer);
    }

    public void onClickExplore()
    {
        LayerManager.showLayer(Consts.Layer.ReadyFindLayer);
    }

    void setCurTime()
    {
        text_time.text = day + "Day  " + (int)curTime + ":00";
    }

    public void addTime(float hour)
    {
        curTime += hour;
        setCurTime();

        // 建造和制作时间
        for(int i = 0; i < list_item.Count; i++)
        {
            list_item[i].addTime(hour);
        }

        // 到了12点自动进入探索
        if((curTime >= 12) && (curTime < 13))
        {
            if(!isBeyond12)
            {
                isBeyond12 = true;
                onClickExplore();
            }
        }
        // 到了19点自动进入夜晚
        else if ((curTime >= 19) && (curTime < 20))
        {
            if (!isBeyond19)
            {
                isBeyond19 = true;
                LayerManager.showLayer(Consts.Layer.ReadyNightLayer);

                if(ReadyFindLayer.s_instance)
                {
                    ReadyFindLayer.s_instance.onClickBack();
                }

                if (MapLayer.s_instance)
                {
                    MapLayer.s_instance.close();
                }

                if (FindLayer.s_instance)
                {
                    FindLayer.s_instance.onClickOk();
                }
            }
        }
    }

    public void newDay()
    {
        ++day;
        curTime = 6;
        isBeyond12 = false;
        isBeyond19 = false;

        setCurTime();

        // 人物属性扣除
        {
            for(int i = 0; i < list_player.Count; i++)
            {
                list_player[i].changePlayerSelfData(Consts.PlayerInfo.Energy, -20);
                list_player[i].changePlayerSelfData(Consts.PlayerInfo.Power, -20);
            }
        }
    }
}
