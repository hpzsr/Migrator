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

    void Start()
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
}
