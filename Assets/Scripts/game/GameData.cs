using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PairData
{
    public int key;
    public int value;

    public PairData(int _key,int _value)
    {
        key = _key;
        value = _value;
    }
}

public class GameData
{
    public List<PairData> myBagsList = new List<PairData>();
    public List<PairData> curGetRewardList = new List<PairData>();

    public static GameData s_instance = null;
    public static GameData getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new GameData();
            s_instance.init();
        }

        return s_instance;
    }

    void init()
    {
        // 背包
        {
            for (int i = 0; i < MaterialsEntity.getInstance().dataList.Count; i++)
            {
                //myBagsList.Add(new PairData(MaterialsEntity.getInstance().dataList[i].id, 2000));
            }
        }
    }

    public int getBagItemCount(int id)
    {
        for (int i = 0; i < myBagsList.Count; i++)
        {
            if(myBagsList[i].key == id)
            {
                return myBagsList[i].value;
            }
        }

        return 0;
    }

    public void changeBagItemCount(int id,int count)
    {
        for (int i = 0; i < myBagsList.Count; i++)
        {
            if (myBagsList[i].key == id)
            {
                myBagsList[i].value += count;
                return;
            }
        }

        if (count > 0)
        {
            myBagsList.Add(new PairData(id, count));
        }
    }
}
