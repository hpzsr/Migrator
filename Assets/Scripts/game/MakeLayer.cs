using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class MakeLayer : LayerBase
{
    public GameObject materialDemo;
    public GameObject MakeItemDemo;
    public Transform materialList;
    public Transform makeList;
    public Text partName;
    public Transform img_time;
    public Transform buff;

    ItemScript itemScript = null;
    Transform curTrans = null;
    List<PairData> needmaterialList = new List<PairData>();

    int curChoiceMakeItem;

    void Start()
    {
        curTrans = startData.data_transform;
        itemScript = curTrans.GetComponent<ItemScript>();

        curChoiceMakeItem = int.Parse(PartsEntity.getInstance().getDataById(itemScript.id, itemScript.level).value.Split(':')[0]);
        setCanMakeItem();
        setMaterialList(curChoiceMakeItem);
    }

    void setCanMakeItem()
    {
        for (int i = 0; i < makeList.childCount; i++)
        {
            Destroy(makeList.GetChild(i).gameObject);
        }

        PartsData base_data = PartsEntity.getInstance().getDataById(itemScript.id, itemScript.level);
        if (base_data != null)
        {
            partName.text = base_data.name;
        }

        string foods = PartsEntity.getInstance().getDataById(itemScript.id,PartsEntity.getInstance().getMaxLevel(itemScript.id)).value;
        string[] str_food_array = foods.Split(':');
        for (int i = 0; i < str_food_array.Length; i++)
        {
            int id = int.Parse(str_food_array[i]);

            GameObject obj = GameObject.Instantiate(MakeItemDemo,makeList);

            Image icon = obj.transform.Find("icon").GetComponent<Image>();
            icon.sprite = CommonUtil.getSprite("Images/" + id);
            //icon.SetNativeSize();

            Text text_unlockLevel = obj.transform.Find("unlockLevel").GetComponent<Text>();
            Button btn = obj.GetComponent<Button>();
            if (itemScript.level >= (i + 1))
            {
                text_unlockLevel.transform.localScale = new Vector3(0, 0, 0);
                btn.onClick.AddListener(()=>
                {
                    curChoiceMakeItem = id;
                    setCanMakeItem();
                    setMaterialList(curChoiceMakeItem);
                });
            }
            else
            {
                text_unlockLevel.transform.localScale = new Vector3(1,1,1);
                text_unlockLevel.text = "LV." + (i + 1);
                btn.enabled = false;
            }

            if(curChoiceMakeItem == id)
            {
                obj.GetComponent<Image>().color = new Color(0,1,0,1);
            }
            else
            {
                obj.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        // 建造时间
        //img_time.Find("text").GetComponent<Text>().text = next_data.createtime + "h";

        // 作用
        //{
        //    string str_value = next_data.value;

        //    if (curTrans.tag == "bed")
        //    {
        //        buff.localScale = new Vector3(1, 1, 1);

        //        string[] str_value_array = str_value.Split(';');
        //        if (str_value_array.Length >= 1)
        //        {
        //            int id = int.Parse(str_value_array[0].Split(':')[0]);
        //            int count = int.Parse(str_value_array[0].Split(':')[1]);
        //            transform.Find("bg/buff/buff1").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + id);
        //            transform.Find("bg/buff/buff1/text").GetComponent<Text>().text = "+" + count + "/h";
        //        }
        //        if (str_value_array.Length >= 2)
        //        {
        //            int id = int.Parse(str_value_array[1].Split(':')[0]);
        //            int count = int.Parse(str_value_array[1].Split(':')[1]);
        //            transform.Find("bg/buff/buff2").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + id);
        //            transform.Find("bg/buff/buff2/text").GetComponent<Text>().text = "+" + count + "/h";
        //        }
        //    }
        //    else if ((curTrans.tag == "kitchen") || (curTrans.tag == "workbench"))
        //    {
        //        buff.localScale = new Vector3(0, 0, 0);

        //    }
        //}
    }

    void setMaterialList(int makeid)
    {
        for (int i = 0; i < materialList.childCount; i++)
        {
            Destroy(materialList.GetChild(i).gameObject);
        }
        needmaterialList.Clear();

        if(PartsEntity.getInstance().getDataById(itemScript.id, itemScript.level) == null)
        {
            Debug.Log("获取数据失败：" + itemScript.id +"  "+ itemScript.level);
            return;
        }

        string str_need = "";
        if (curTrans.tag == "kitchen")
        {
            str_need = MaterialsEntity.getInstance().getDataById(makeid).hechengitem;
        }
        else if (curTrans.tag == "workbench")
        {
            str_need = PartsEntity.getInstance().getDataById(makeid,1).material;
        }
        string[] str_need_array = str_need.Split(';');
        for (int i = 0; i < str_need_array.Length; i++)
        {
            string[] id_count = str_need_array[i].Split(':');
            int id = int.Parse(id_count[0]);
            int need_count = int.Parse(id_count[1]);
            int have_count = GameData.getInstance().getBagItemCount(id);

            needmaterialList.Add(new PairData(id, need_count));

            // 需要的材料icon和数量
            {
                GameObject obj = GameObject.Instantiate(materialDemo, materialList);

                Image icon = obj.transform.Find("icon").GetComponent<Image>();
                icon.sprite = CommonUtil.getSprite("Images/" + id);
                icon.SetNativeSize();

                Text need = obj.transform.Find("need").GetComponent<Text>();
                need.text = have_count + "/" + need_count;
                if (have_count < need_count)
                {
                    need.color = new Color(1, 0, 0, 1);
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
        if(GameData.getInstance().getUnLockPartById(curChoiceMakeItem) != null)
        {
            return;
        }

        for (int i = 0; i < needmaterialList.Count; i++)
        {
            GameData.getInstance().changeBagItemCount(needmaterialList[i].key, -needmaterialList[i].value);
        }

        GameData.getInstance().changeBagItemCount(curChoiceMakeItem, 1);
        setMaterialList(curChoiceMakeItem);

        if (curTrans.tag == "kitchen")
        {
            ToastScript.show(MaterialsEntity.getInstance().getDataById(curChoiceMakeItem).name + " +1");
        }
        else if (curTrans.tag == "workbench")
        {
            GameData.getInstance().addPart(curChoiceMakeItem);
            ToastScript.show("Unlock " + PartsEntity.getInstance().getDataById(curChoiceMakeItem,1).name);
        }
    }
}
