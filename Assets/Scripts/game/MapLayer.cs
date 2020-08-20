using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLayer : LayerBase
{
    public Text text_tili;
    public Transform player;
    public Transform squares;
    public Transform searchPoints;
    public GameObject searchPoint;
    public Image img_map;
    public Image img_atkRange;
    public Image img_player;

    int player_id;
    int curMap = 2;
    int tili = 100;
    int curInSquare = 1;
    int atkRangeSquare = 0;
    List<Transform> searchPoint_list = new List<Transform>();
    ItemScript nextFloor = null;
    ItemScript enemy = null;

    public static MapLayer s_instance = null;

    void Start()
    {
        s_instance = this;

        initMap(1);

        curMap = startData.data_int;
        player_id = int.Parse(startData.data_string);
        tili = PlayerEntity.getInstance().getDataById(player_id).energy;
        img_player.sprite = CommonUtil.getSprite("Images/player" + player_id);
        img_player.SetNativeSize();

        // 方格点击事件
        for (int i = 0; i < squares.childCount; i++)
        {
            Button btn = squares.GetChild(i).GetComponent<Button>();
            if (btn)
            {
                btn.onClick.AddListener(() =>
                {
                    if (btn.GetComponent<Image>().color.r > 0)
                    {
                        changeTiLi(-1);
                        moveTo(btn.transform);
                    }
                    else if (btn.GetComponent<Image>().color.g > 0)
                    {
                        hideRedSquare();
                        btn.GetComponent<Image>().color = new Color(1,0,0,0.5f);
                    }
                    else
                    {
                        // 人物站在搜索点上
                        if((player.localPosition == btn.transform.localPosition))
                        {
                            for (int j = 0; j < searchPoint_list.Count; j++)
                            {
                                if (searchPoint_list[j].localPosition == btn.transform.localPosition)
                                {
                                    var data = new Event.EventCallBackData();
                                    data.data_transform = searchPoint_list[j];
                                    data.data_bool = false;
                                    data.data_string = player_id + "";
                                    LayerManager.showLayer(Consts.Layer.FindLayer, data);
                                    break;
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                Debug.Log(squares.GetChild(i).name);
            }
        }
        
        showCanToSquare();

        // 扣除进入地图体力
        {
            int tili = FamilyLayer.s_instance.getPlayerScriptById(player_id).getPlayerSelfDataByType(Consts.PlayerInfo.Energy);
            List<MapData> list = MapEntity.getInstance().getDataByMapFloor(curMap, 1);
            if (list.Count > 0)
            {
                changeTiLi(-list[0].energy);
            }
        }
    }
    
    void Update()
    {
        
    }

    void changeTiLi(int value)
    {
        FamilyLayer.s_instance.getPlayerScriptById(player_id).changePlayerSelfData(Consts.PlayerInfo.Energy, value);
        tili += value;
        text_tili.text = tili.ToString();
    }

    void initMap(int floor)
    {
        // 初始化 清空之前的
        {
            curInSquare = 1;
            player.localPosition = squares.GetChild(0).localPosition;

            for (int i = 0; i < searchPoint_list.Count; i++)
            {
                Destroy(searchPoint_list[i].gameObject);
            }
            searchPoint_list.Clear();

            if (nextFloor)
            {
                Destroy(nextFloor.gameObject);
                nextFloor = null;
            }

            if (enemy)
            {
                Destroy(enemy.gameObject);
                enemy = null;
            }
        }

        List<MapData> list = MapEntity.getInstance().getDataByMapFloor(curMap,floor);

        // 添加搜索点
        for (int i = 0; i < list.Count; i++)
        {
            addSearchPoint(list[i].point);
        }

        // 添加脚印
        if (list[0].nextFloorPoint != 0)
        {
            setFloorPoint(floor, list[0].nextFloorPoint);
        }

        // 添加敌人
        if (list[0].enemy != 0)
        {
            addEnemy(floor, list[0].enemy);
            setAtkRange(floor, list[0].enemy);
        }

        showCanToSquare();
    }

    void addSearchPoint(int square)
    {
        GameObject obj = GameObject.Instantiate(searchPoint, searchPoints);
        obj.transform.localPosition = squares.Find("square_" + square).localPosition;
        obj.transform.name = "find_" + curMap + "_" + square;
        obj.GetComponent<ItemScript>().init(false);

        searchPoint_list.Add(obj.transform);
    }

    void addEnemy(int floor, int square)
    {
        GameObject obj = GameObject.Instantiate(searchPoint, searchPoints);
        obj.transform.tag = "enemy";
        obj.transform.localPosition = squares.Find("square_" + square).localPosition;
        obj.transform.name = "find_" + curMap + "_" + square;
        obj.GetComponent<Image>().sprite = CommonUtil.getSprite("Images/monster");
        obj.GetComponent<Image>().SetNativeSize();

        enemy = obj.GetComponent<ItemScript>();
        enemy.init(false);
        enemy.floor = floor;
    }

    void setFloorPoint(int floor,int square)
    {
        img_map.sprite = CommonUtil.getSprite("Images/mapfloor_" + floor);

        GameObject obj = GameObject.Instantiate(searchPoint, searchPoints);
        obj.transform.tag = "jiaoyin";
        obj.transform.localPosition = squares.Find("square_" + square).localPosition;
        obj.transform.name = "find_" + curMap + "_" + square;
        obj.GetComponent<Image>().sprite = CommonUtil.getSprite("Images/nextlevel");
        obj.GetComponent<Image>().SetNativeSize();

        nextFloor = obj.GetComponent<ItemScript>();
        nextFloor.init(false);
        nextFloor.floor = floor;
    }

    void setAtkRange(int floor, int square)
    {
        atkRangeSquare = square;
        img_atkRange.transform.localPosition = squares.Find("square_" + square).localPosition;
    }

    // 移动到指定地点
    void moveTo(Transform targetTrans)
    {
        for (int i = 0; i < squares.childCount; i++)
        {
            Transform trans = squares.GetChild(i);
            Image img = trans.GetComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
        }

        int num = int.Parse(targetTrans.name.Split('_')[1]);
        curInSquare = num;
        player.GetComponent<Image>().rectTransform.DOAnchorPos(targetTrans.localPosition, 0.5f).SetEase(Ease.Linear).OnComplete(()=>
        {
            checkIsAtk();

            showCanToSquare();
            
            for (int i = 0; i < searchPoint_list.Count; i++)
            {
                if (searchPoint_list[i].localPosition == targetTrans.transform.localPosition)
                {
                    var data = new Event.EventCallBackData();
                    data.data_transform = searchPoint_list[i];
                    data.data_bool = false;
                    data.data_string = player_id + "";
                    LayerManager.showLayer(Consts.Layer.FindLayer, data);
                    return;
                }
            }

            if(nextFloor)
            {
                if (nextFloor.transform.localPosition == targetTrans.transform.localPosition)
                {
                    initMap(nextFloor.floor + 1);
                }
            }
        });
    }

    void checkIsAtk()
    {
        if((curInSquare == atkRangeSquare) ||
           (curInSquare == (atkRangeSquare - 1)) ||
           (curInSquare == (atkRangeSquare + 1)) ||
           (curInSquare == (atkRangeSquare + 6)) ||
           (curInSquare == (atkRangeSquare - 6)) ||
           (curInSquare == (atkRangeSquare + 5)) ||
           (curInSquare == (atkRangeSquare - 5)) ||
           (curInSquare == (atkRangeSquare + 7)) ||
           (curInSquare == (atkRangeSquare - 7)))
        {
            changeTiLi(-1);
        }
    }

    void showCanToSquare()
    {
        for(int i = 0; i < squares.childCount; i++)
        {
            Transform trans = squares.GetChild(i);
            Image img = trans.GetComponent<Image>();
            int num = int.Parse(trans.name.Split('_')[1]);
            if(Mathf.Abs(curInSquare - num) == 6)
            {
                img.color = new Color(0,1,0,0.5f);
            }
            else if (Mathf.Abs(curInSquare - num) == 1)
            {
                if (((curInSquare - 1) % 6 == 0) && (num == (curInSquare - 1)))
                {
                }
                else if ((curInSquare % 6 == 0) && (num == (curInSquare + 1)))
                {
                }
                else
                {
                    img.color = new Color(0, 1, 0, 0.5f);
                }
            }
            else
            {
                img.color = new Color(0, 0, 0, 0);
            }
        }
    }

    void hideRedSquare()
    {
        for (int i = 0; i < squares.childCount; i++)
        {
            Transform trans = squares.GetChild(i);
            Image img = trans.GetComponent<Image>();
            if (img.color.r > 0)
            {
                img.color = new Color(0, 1, 0, 0.5f);
            }
        }
    }

    public void close()
    {
        s_instance = null;
        Destroy(gameObject);
    }

    public void onClickBack()
    {
        // 把搜索的物品添加到背包
        var getList = GameData.getInstance().curGetRewardList;
        for (int i = 0; i < getList.Count; i++)
        {
            GameData.getInstance().changeBagItemCount(getList[i].key, getList[i].value);
        }

        s_instance = null;
        Destroy(gameObject);

        LayerManager.showLayer(Consts.Layer.BackHomeLayer);
    }
}
