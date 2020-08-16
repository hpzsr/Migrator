using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildLayer : LayerBase
{
    public GameObject materialDemo;
    public Transform materialList;
    public Text partName;
    public Image old_icon;
    public Image new_icon;
    public Transform img_time;
    public Transform buff;
    public Transform unlock;
    public Transform fullLevel;

    ItemScript itemScript = null;
    Transform curTrans = null;
    List<PairData> needmaterialList = new List<PairData>();
    bool isCanUpgrade = true;

    void Start()
    {
        curTrans = startData.data_transform;
        itemScript = curTrans.GetComponent<ItemScript>();

        setMaterialList();
    }
    
    void setMaterialList()
    {
        for(int i = 0; i < materialList.childCount; i++)
        {
            Destroy(materialList.GetChild(i).gameObject);
        }
        needmaterialList.Clear();

        int curLevel = itemScript.level;
        checkUpgrade();

        PartsData base_data = PartsEntity.getInstance().getDataById(itemScript.id, 1);
        if(base_data != null)
        {
            partName.text = base_data.name;
        }
        PartsData next_data = PartsEntity.getInstance().getDataById(itemScript.id, curLevel + 1);
        if (next_data == null)
        {
            //Debug.Log("next_materialsData == null  id= " + itemScript.id + "  level= " + (curLevel + 1));
            return;
        }

        // 建造时间
        img_time.localScale = new Vector3(1,1,1);
        img_time.Find("text").GetComponent<Text>().text = next_data.createtime + "h";

        // 作用
        {
            string str_value = next_data.value;
            
            if (curTrans.tag == "bed")
            {
                buff.localScale = new Vector3(1,1,1);
                unlock.localScale = new Vector3(0, 0, 0);

                string[] str_value_array = str_value.Split(';');
                if(str_value_array.Length >= 1)
                {
                    int id = int.Parse(str_value_array[0].Split(':')[0]);
                    int count = int.Parse(str_value_array[0].Split(':')[1]);
                    transform.Find("bg/buff/buff1").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + id);
                    transform.Find("bg/buff/buff1/text").GetComponent<Text>().text = "+" + count + "/h";
                }
                if (str_value_array.Length >= 2)
                {
                    int id = int.Parse(str_value_array[1].Split(':')[0]);
                    int count = int.Parse(str_value_array[1].Split(':')[1]);
                    transform.Find("bg/buff/buff2").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + id);
                    transform.Find("bg/buff/buff2/text").GetComponent<Text>().text = "+" + count + "/h";
                }
            }
            else if ((curTrans.tag == "kitchen") || (curTrans.tag == "workbench"))
            {
                unlock.localScale = new Vector3(1,1,1);
                buff.localScale = new Vector3(0, 0, 0);

                string[] str_value_array = str_value.Split(':');
                for (int i = 1; i < unlock.childCount; i++)
                {
                    unlock.GetChild(i).localScale = new Vector3(0, 0, 0);
                }

                for (int i = 0; i < str_value_array.Length; i++)
                {
                    unlock.GetChild(i + 1).localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    Image image = unlock.GetChild(i + 1).GetComponent<Image>();
                    image.sprite = CommonUtil.getSprite("Images/" + str_value_array[i]);
                    image.SetNativeSize();
                }
            }
        }

        old_icon.sprite = CommonUtil.getSprite("Images/" + itemScript.id + "_" + curLevel);
        new_icon.sprite = CommonUtil.getSprite("Images/" + itemScript.id + "_" + (curLevel +1));
        old_icon.SetNativeSize();
        new_icon.SetNativeSize();
        FamilyLayer.s_instance.setColorByLevel(old_icon, curLevel);
        FamilyLayer.s_instance.setColorByLevel(new_icon, curLevel + 1);

        string str_need = next_data.material;
        string[] str_need_array = str_need.Split(';');

        for (int i = 0; i < str_need_array.Length; i++)
        {
            string[] id_count = str_need_array[i].Split(':');
            int id = int.Parse(id_count[0]);
            int need_count = int.Parse(id_count[1]);
            int have_count = GameData.getInstance().getBagItemCount(id);
            if(have_count < need_count)
            {
                isCanUpgrade = false;
            }
            
            needmaterialList.Add(new PairData(id, need_count));

            // 需要的材料icon和数量
            {
                GameObject obj = GameObject.Instantiate(materialDemo, materialList);

                Image icon = obj.transform.Find("icon").GetComponent<Image>();
                icon.sprite = CommonUtil.getSprite("Images/" + id);
                icon.SetNativeSize();

                Text need = obj.transform.Find("need").GetComponent<Text>();
                need.text = have_count + "/" + need_count;
                if(have_count < need_count)
                {
                    need.color = new Color(1,0,0,1);
                }
                else
                {
                    need.color = new Color(0, 0, 0, 1);
                }
            }
        }
    }

    public void onClickClose()
    {
        Destroy(gameObject);
    }

    public void onClickBuild()
    {
        if(isCanUpgrade)
        {
            for(int i = 0; i < needmaterialList.Count; i++)
            {
                GameData.getInstance().changeBagItemCount(needmaterialList[i].key, -needmaterialList[i].value);
            }
            
            itemScript.addItemLevel(1);
            setMaterialList();

            checkUpgrade();
        }
    }

    public void checkUpgrade()
    {
        if (itemScript.level >= PartsEntity.getInstance().getMaxLevel(itemScript.id))
        {
            transform.Find("bg/btn_build").localScale = new Vector3(0, 0, 0);
            fullLevel.localScale = new Vector3(1,1,1);
        }
        else
        {
            transform.Find("bg/btn_build").localScale = new Vector3(1,1,1);
            fullLevel.localScale = new Vector3(0,0,0);
        }
    }
}
