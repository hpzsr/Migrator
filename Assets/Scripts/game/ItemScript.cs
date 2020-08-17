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
                FamilyLayer.s_instance.curControlPlayer.moveTo(true, transform.localPosition, transform);
            });
        }

        if ((transform.tag != "ladder") && isSetPos)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(floor), 0);
        }
    }

    public void setItemLevel(int _level)
    {
        level = _level;
        FamilyLayer.s_instance.setColorByLevel(self_img, level);
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
}
