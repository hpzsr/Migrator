using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    public Image self_img;
    public int floor;
    public int id = -1;
    public int level = 1;
    public bool isCanUpgrade = false;

    public List<PairData> rewardList = new List<PairData>();

    Transform btn_upgrade = null;

    // 升级相关
    Transform progress_bg;
    Image img_progress;
    float curBuildNeedTime = 10;
    float curBuildPassTime = 0;
    public Consts.PartState partState = Consts.PartState.Normal;

    // 制作相关
    public int curMakeType = 0;
    public float curMakeNeedTime = 10;
    public float curMakePassTime = 0;
    public Consts.PartMakeState partMakeState = Consts.PartMakeState.Normal;

    void Start()
    {
        btn_upgrade = transform.Find("btn_upgrade");
        progress_bg = transform.Find("progress_bg");
        if (progress_bg)
        {
            img_progress = transform.Find("progress_bg/progress").GetComponent<Image>();
            progress_bg.localScale = new Vector3(0, 0, 0);
        }
    }

    void Update()
    {
    }

    public void init(bool isSetPos = true)
    {
        self_img = GetComponent<Image>();

        if (transform.tag == "bed")
        {
            id = 301;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "kitchen")
        {
            id = 302;
            isCanUpgrade = true;
            setItemLevel(0);

            // 食物
            setCanEatFoods();
        }
        else if (transform.tag == "heater")
        {
            id = 303;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "workbench")
        {
            id = 304;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "toy")
        {
            id = 305;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "vase")
        {
            id = 306;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "gitar")
        {
            id = 307;
            isCanUpgrade = true;
            setItemLevel(0);
        }
        else if (transform.tag == "find")
        {
            int map = int.Parse(transform.name.Split('_')[1]);
            int point = int.Parse(transform.name.Split('_')[2]);

            MapData mapData = MapEntity.getInstance().getDataByMap(map, point);

            string str_reward = mapData.Reward;
            string[] str_reward_array = str_reward.Split(';');

            for (int i = 0; i < str_reward_array.Length; i++)
            {
                string[] id_count = str_reward_array[i].Split(':');
                int id = int.Parse(id_count[0]);
                int count = int.Parse(id_count[1]);
                rewardList.Add(new PairData(id, count));
            }
        }

        if(transform.Find("btn_upgrade") != null)
        {
            Button btn_upgrade = transform.Find("btn_upgrade").GetComponent<Button>();
            btn_upgrade.onClick.AddListener(()=>
            {
                FamilyLayer.s_instance.curControlPlayer.moveTo(Consts.MoveEndEvent.Upgrade, transform.localPosition, transform);
            });
        }

        if ((transform.tag != "ladder") && isSetPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(floor), 0);
        }
    }

    public void setItemLevel(int _level)
    {
        // 玩偶、花瓶、吉他
        if(id >= 305 && id <= 307)
        {
            level = _level;
            self_img.sprite = CommonUtil.getSprite("Images/" + id);
            self_img.SetNativeSize();
            FamilyLayer.s_instance.setColorByLevel(self_img, level);
        }
        else
        {
            level = _level;
            self_img.sprite = CommonUtil.getSprite("Images/" + id + "_" + level);
            self_img.SetNativeSize();
            FamilyLayer.s_instance.setColorByLevel(self_img, level);
        }
    }

    public void addItemLevel(int _level)
    {
        level += _level;
        setItemLevel(level);
    }

    public void startMake(int type)
    {
        curMakeType = type;
        if (transform.tag == "kitchen")
        {
            curMakeNeedTime = int.Parse(MaterialsEntity.getInstance().getDataById(type).cooktime);
        }
        else if (transform.tag == "workbench")
        {
            curMakeNeedTime = PartsEntity.getInstance().getDataById(type, 1).createtime;
        }
        
        curMakeNeedTime = 0.1f;
        partMakeState = Consts.PartMakeState.Making;
    }

    public void stopMake()
    {
        curMakePassTime = 0;
        partMakeState = Consts.PartMakeState.Normal;
        GameData.getInstance().changeBagItemCount(curMakeType, 1);

        if (transform.tag == "kitchen")
        {
            setCanEatFoods();
            ToastScript.show(MaterialsEntity.getInstance().getDataById(curMakeType).name + " +1");
        }
        else if (transform.tag == "workbench")
        {
            GameData.getInstance().addPart(curMakeType);
            ToastScript.show("Unlock " + PartsEntity.getInstance().getDataById(curMakeType, 1).name);
        }
    }

    public void startBuild()
    {
        curBuildNeedTime = PartsEntity.getInstance().getDataById(id, level + 1).createtime;
        curBuildNeedTime = 0.1f;
        partState = Consts.PartState.Building;
    }

    public void stopBuild()
    {
        curBuildPassTime = 0;
        partState = Consts.PartState.Normal;
        progress_bg.localScale = new Vector3(0, 0, 0);
    }

    public void addTime(float hour)
    {
        if (checkHasPlayerAround())
        {
            if (partState == Consts.PartState.Building)
            {
                curBuildPassTime += hour;
                setBuildProgress(curBuildPassTime / curBuildNeedTime);
                //btn_upgrade.localScale = new Vector3(0,0,0);
            }
            else
            {
                if (btn_upgrade)
                {
                    //btn_upgrade.localScale = new Vector3(1, 1, 1);
                }
            }

            if (partMakeState == Consts.PartMakeState.Making)
            {
                curMakePassTime += hour;
                if (curMakePassTime >= curMakeNeedTime)
                {
                    stopMake();
                }
            }
        }
    }

    bool checkHasPlayerAround()
    {
        for(int i = 0; i < FamilyLayer.s_instance.list_player.Count; i++)
        {
            if(CommonUtil.twoObjDistance_2D(transform.localPosition, FamilyLayer.s_instance.list_player[i].transform.localPosition) <= 10)
            {
                return true;
            }
        }

        return false;
    }

    public void changeRewardListData(int id, int count)
    {
        for (int i = 0; i < rewardList.Count; i++)
        {
            if (rewardList[i].key == id)
            {
                rewardList[i].value += count;
                if (rewardList[i].value <= 0)
                {
                    rewardList.RemoveAt(i);
                }
                return;
            }
        }

        if (count > 0)
        {
            rewardList.Add(new PairData(id, count));
        }
    }
    
    public void setCanEatFoods()
    {
        List<int> list = new List<int>();
        if (GameData.getInstance().getBagItemCount(209) > 0)
        {
            list.Add(209);
        }
        if (GameData.getInstance().getBagItemCount(210) > 0)
        {
            list.Add(210);
        }
        if (GameData.getInstance().getBagItemCount(211) > 0)
        {
            list.Add(211);
        }

        if(list.Count > 0)
        {
            transform.Find("foods").localScale = new Vector3(1, 1, 1);
            for (int i = 1; i <= 3; i++)
            {
                Button btn = transform.Find("foods/food_" + i).GetComponent<Button>();
                if (i <= list.Count)
                {
                    btn.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
                    btn.transform.localScale = new Vector3(1, 1, 1);

                    int food_id = list[i - 1];

                    Image icon = btn.transform.GetComponent<Image>();
                    icon.sprite = CommonUtil.getSprite("Images/" + list[i - 1]);

                    Text text_count = btn.transform.Find("count").GetComponent<Text>();
                    text_count.text = "x"+GameData.getInstance().getBagItemCount(list[i - 1]).ToString();
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(()=>
                    {
                        FamilyLayer.s_instance.curControlPlayer.moveTo((Consts.MoveEndEvent)food_id, transform.localPosition,transform);
                    });
                }
                else
                {
                    btn.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                    btn.transform.localScale = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            transform.Find("foods").localScale = new Vector3(0, 0, 0);
        }
    }

    void setBuildProgress(float _progress)
    {
        if (_progress >= 1)
        {
            stopBuild();
            addItemLevel(1);
        }
        else
        {
            progress_bg.localScale = new Vector3(1, 1, 1);
        }
        img_progress.fillAmount = _progress;
    }
}
