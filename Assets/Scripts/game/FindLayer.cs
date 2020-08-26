using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class FindLayer : LayerBase
{
    public Transform getGroup;
    public Transform waitGetGroup;

    public Button btn_getAll;
    public Button btn_get;
    public Button btn_giveup;
    public Button btn_ok;
    
    List<PairData> waitGetList = new List<PairData>();
    List<PairData> getFromCurPointList = new List<PairData>();

    Transform curChoiceItem_get = null;
    Transform curChoiceItem_waitGet = null;

    ItemScript itemScript = null;

    public static FindLayer s_instance = null;

    bool isFromFamilyLayer = false;
    int canUseSquareCount = 10;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            for(int i = 0; i < getFromCurPointList.Count; i++)
            {
                Debug.Log(getFromCurPointList[i].key + "  " + getFromCurPointList[i].value);
            }
        }
    }

    void Start()
    {
        s_instance = this;

        isFromFamilyLayer = startData.data_bool;
        if (isFromFamilyLayer)
        {
            GameData.getInstance().curGetRewardList.Clear();
        }

        Debug.Log("搜索人物：" + startData.data_string);
        int player_id = int.Parse(startData.data_string);
        canUseSquareCount = PlayerEntity.getInstance().getDataById(player_id).bagCount;

        // 禁用格子
        for (int i = getGroup.childCount - 1; i >= canUseSquareCount; i--)
        {
            Destroy(getGroup.GetChild(i).gameObject);
        }

        // 左边
        for (int i = 0; i < getGroup.childCount; i++)
        {
            Transform temp = getGroup.GetChild(i);
            temp.GetComponent<Button>().onClick.AddListener(()=>
            {
                if (temp.name.Split('_').Length == 2)
                { 
                    curChoiceItem_get = temp;
                    setCurChoiceItem_Get(curChoiceItem_get);
                }
            });

            int index = i;
            temp.Find("delete").GetComponent<Button>().onClick.AddListener(() =>
            {
                int id = int.Parse(temp.name.Split('_')[0]);
                int count = 1;
                int realChangeCount = changeGetListData(id, -count, index);
                changeWaitGetListData(id, -realChangeCount);
                changeGetFromCurPointListData(id, Mathf.Abs(realChangeCount) > curPointGetCountById(id) ? -curPointGetCountById(id) : realChangeCount);
                refreshGroup();
            });
        }

        // 右边
        for (int i = 0; i < waitGetGroup.childCount; i++)
        {
            Transform temp = waitGetGroup.GetChild(i);
            temp.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (temp.name.Split('_').Length == 2)
                {
                    curChoiceItem_waitGet = temp;
                    setCurChoiceItem_WaitGet(curChoiceItem_waitGet);
                }
            });

            temp.Find("add").GetComponent<Button>().onClick.AddListener(() =>
            {
                int id = int.Parse(temp.name.Split('_')[0]);
                int count = 1;
                int realChangeCount = changeGetListData(id, count);
                changeWaitGetListData(id, -realChangeCount);
                changeGetFromCurPointListData(id, realChangeCount);
                refreshGroup();
            });
        }

        itemScript = startData.data_transform.GetComponent<ItemScript>();
        if (itemScript)
        {
            for (int i = 0; i < itemScript.rewardList.Count; i++)
            {
                waitGetList.Add(new PairData(itemScript.rewardList[i].key, itemScript.rewardList[i].value));
            }
        }
        else
        {
            Debug.Log("----itemScript==null    " + startData.data_transform.name);
        }

        refreshGroup();
        setCurChoiceItem_Get(curChoiceItem_get);
        setCurChoiceItem_WaitGet(curChoiceItem_waitGet);
    }

    void refreshGroup()
    {
        var getList = GameData.getInstance().curGetRewardList;
        // 左边
        {
            for (int i = 0; i < getList.Count; i++)
            {
                Transform temp = getGroup.GetChild(i);
                temp.Find("icon").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + getList[i].key);
                temp.Find("count").GetComponent<Text>().text = getList[i].value.ToString();

                if ((temp == curChoiceItem_get) && (temp.name.Split('_')[0] != getList[i].key.ToString()))
                {
                    setCurChoiceItem_Get(getList.Count > 0 ? getGroup.GetChild(0) : null);
                }
                temp.name = getList[i].key + "_" + getList[i].value;
            }

            for (int i = getList.Count; i < getGroup.childCount; i++)
            {
                Transform temp = getGroup.GetChild(i);
                temp.Find("icon").GetComponent<Image>().sprite = null;
                temp.Find("count").GetComponent<Text>().text = "0";
                temp.name = "item" + (i + 1);

                if (temp == curChoiceItem_get)
                {
                    setCurChoiceItem_Get(getList.Count > 0 ? getGroup.GetChild(0) : null);
                }
            }
        }

        // 右边
        {
            // 有奖励
            for (int i = 0; i < waitGetList.Count; i++)
            {
                Transform temp = waitGetGroup.GetChild(i);
                temp.Find("icon").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + waitGetList[i].key);
                temp.Find("count").GetComponent<Text>().text = waitGetList[i].value.ToString();

                if ((temp == curChoiceItem_waitGet) && (temp.name.Split('_')[0] != waitGetList[i].key.ToString()))
                {
                    setCurChoiceItem_WaitGet(waitGetList.Count > 0 ? waitGetGroup.GetChild(0) : null);
                }
                temp.name = waitGetList[i].key + "_" + waitGetList[i].value;
            }

            // 空格
            for (int i = waitGetList.Count; i < waitGetGroup.childCount; i++)
            {
                Transform temp = waitGetGroup.GetChild(i);
                temp.Find("icon").GetComponent<Image>().sprite = null;
                temp.Find("count").GetComponent<Text>().text = "0";
                temp.name = "item" + (i + 1);

                if(temp == curChoiceItem_waitGet)
                {
                    setCurChoiceItem_WaitGet(waitGetList.Count > 0 ? waitGetGroup.GetChild(0) : null);
                }
            }
        }
    }

    void setCurChoiceItem_Get(Transform trans)
    {
        curChoiceItem_get = trans;
        for(int i = 0; i < getGroup.childCount; i++)
        {
            if(curChoiceItem_get == getGroup.GetChild(i))
            {
                getGroup.GetChild(i).Find("delete").localScale = new Vector3(1,1,1);
            }
            else
            {
                getGroup.GetChild(i).Find("delete").localScale = new Vector3(0,0,0);
            }
        }
    }

    void setCurChoiceItem_WaitGet(Transform trans)
    {
        curChoiceItem_waitGet = trans;
        for (int i = 0; i < waitGetGroup.childCount; i++)
        {
            if (curChoiceItem_waitGet == waitGetGroup.GetChild(i))
            {
                waitGetGroup.GetChild(i).Find("add").localScale = new Vector3(1, 1, 1);
            }
            else
            {
                waitGetGroup.GetChild(i).Find("add").localScale = new Vector3(0, 0, 0);
            }
        }
    }
     
    int changeGetListData(int id,int count,int index = -1)
    {
        Debug.Log("左边变化：" + id + "  " + count);
        int count_copy = count;

        var getList = GameData.getInstance().curGetRewardList;
        if (count > 0)
        {
            int isAddCount = 0;
            int maxInBag = MaterialsEntity.getInstance().getDataById(id).maxinbag;
            for (int i = 0; i < getList.Count; i++)
            {
                if ((getList[i].key == id) && getList[i].value < maxInBag)
                {
                    // 当前格子塞得下新的
                    if((maxInBag - getList[i].value) >= count)
                    {
                        getList[i].value += count;
                        isAddCount += count;
                    }
                    else
                    {
                        isAddCount += (maxInBag - getList[i].value);
                        getList[i].value += (maxInBag - getList[i].value);
                    }
                    break;
                }
            }

            if (isAddCount < count)
            {
                int restCount = (count - isAddCount);
                for(int i = 0; i < restCount / maxInBag; i++)
                {
                    if(getList.Count >= canUseSquareCount)
                    {
                        break;
                    }
                    getList.Add(new PairData(id, maxInBag));
                    isAddCount += maxInBag;
                }

                if (restCount % maxInBag > 0)
                {
                    if (getList.Count < canUseSquareCount)
                    {
                        getList.Add(new PairData(id, restCount % maxInBag));
                        isAddCount += restCount % maxInBag;
                    }
                }
            }

            return isAddCount;
        }
        else if(count < 0)
        {
            int isDeleteCount = 0;
            if (index >= 0)
            {
                getList[index].value += count;
                if (getList[index].value <= 0)
                {
                    getList.RemoveAt(index);
                }
            }
            else
            {
                for (int i = getList.Count - 1; i >= 0; i--)
                {
                    if (isDeleteCount < Mathf.Abs(count))
                    {
                        if (getList[i].key == id)
                        {
                            if (getList[i].value >= Mathf.Abs(count))
                            {
                                isDeleteCount += Mathf.Abs(count);
                            }
                            else
                            {
                                isDeleteCount += getList[i].value;
                            }

                            getList[i].value += count;
                            if (getList[i].value <= 0)
                            {
                                getList.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return count_copy;
        }

        return 0;
    }
    
    void changeGetFromCurPointListData(int id, int count)
    {
        for (int i = 0; i < getFromCurPointList.Count; i++)
        {
            if (getFromCurPointList[i].key == id)
            {
                getFromCurPointList[i].value += count;
                if (getFromCurPointList[i].value <= 0)
                {
                    getFromCurPointList.RemoveAt(i);
                }
                return;
            }
        }

        if (count > 0)
        {
            getFromCurPointList.Add(new PairData(id, count));
        }
    }

    int curPointGetCountById(int id)
    {
        for (int i = 0; i < getFromCurPointList.Count; i++)
        {
            if (getFromCurPointList[i].key == id)
            {
                return getFromCurPointList[i].value;
            }
        }

        return 0;
    }

    void changeWaitGetListData(int id, int count)
    {
        Debug.Log("右边变化：" + id + "  " + count);
        if (count > 0)
        {
            int canBackCount = curPointGetCountById(id);
            if (canBackCount > 0)
            {
                int changeCount = count > canBackCount ? canBackCount : count;
                for (int i = 0; i < waitGetList.Count; i++)
                {
                    if (waitGetList[i].key == id)
                    {
                        waitGetList[i].value += changeCount;
                        return;
                    }
                }
                waitGetList.Add(new PairData(id, changeCount));
            }
        }
        else if (count < 0)
        {
            for (int i = 0; i < waitGetList.Count; i++)
            {
                if (waitGetList[i].key == id)
                {
                    waitGetList[i].value += count;
                    if (waitGetList[i].value <= 0)
                    {
                        waitGetList.RemoveAt(i);
                    }

                    return;
                }
            }
        }
    }
    
    public void onClickGiveUp()
    {
        if (curChoiceItem_get != null)
        {
            int id = int.Parse(curChoiceItem_get.name.Split('_')[0]);
            int count = int.Parse(curChoiceItem_get.name.Split('_')[1]);
            int realChangeCount = changeGetListData(id, -count, curChoiceItem_get.GetSiblingIndex());
            changeWaitGetListData(id, -realChangeCount);
            changeGetFromCurPointListData(id, Mathf.Abs(realChangeCount) > curPointGetCountById(id) ? -curPointGetCountById(id) : realChangeCount);

            refreshGroup();
        }
    }

    public void onClickOk()
    {
        var getList = GameData.getInstance().curGetRewardList;
        for (int i = 0; i < getList.Count; i++)
        {
            if (isFromFamilyLayer)
            {
                GameData.getInstance().changeBagItemCount(getList[i].key, getList[i].value);
            }
            itemScript.changeRewardListData(getList[i].key, -getList[i].value);
        }

        if(waitGetList.Count == 0)
        {
            Transform white_img = itemScript.transform.Find("white");
            if(white_img)
            {
                white_img.localScale = new Vector3(0,0,0);
            }
        }

        s_instance = null;
        Destroy(gameObject);
    }

    public void onClickGetAll()
    {
        int repeatCount = 0;
        while(waitGetList.Count > 0)
        {
            int id = waitGetList[0].key;
            int realChangeCount = changeGetListData(id, waitGetList[0].value,-1);
            changeWaitGetListData(id, -realChangeCount);
            changeGetFromCurPointListData(id, realChangeCount);

            if(++repeatCount > 10)
            {
                Debug.Log("onClickGetAll----删除次数过多");
                break;
            }
        }
        
        refreshGroup();
    }

    public void onClickGet()
    {
        if(curChoiceItem_waitGet != null)
        {
            int id = int.Parse(curChoiceItem_waitGet.name.Split('_')[0]);
            int count = int.Parse(curChoiceItem_waitGet.name.Split('_')[1]);

            int realChangeCount = changeGetListData(id, count);
            changeWaitGetListData(id, -realChangeCount);
            changeGetFromCurPointListData(id, realChangeCount);

            refreshGroup();
        }
    }
}
