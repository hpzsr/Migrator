using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsData
{
    public int id;
    public string name;
    public string material;
    public int level;
    public string value;
    public int createtime;
    public int power;
}

public class PartsEntity
{
	static PartsEntity s_instance = null;

	public List<PartsData> dataList = new List<PartsData>();

	static public PartsEntity getInstance()
	{
		if(s_instance == null)
		{
			s_instance = new PartsEntity();
			s_instance.init();
		}

		return s_instance;
	}

	void init()
	{
		dataList = JsonUtils.loadJsonToList<PartsData>("parts");
	}

	public PartsData getDataById(int id,int level)
	{
		for(int i = 0; i < dataList.Count; i++)
		{
			if((dataList[i].id == id) && (dataList[i].level == level))
			{
				return dataList[i];
			}
		}

		return null;
	}

    public int getMaxLevel(int id)
    {
        int level = 0;
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i].id == id)
            {
                if(dataList[i].level > level)
                {
                    level = dataList[i].level;
                }
            }
        }

        return level;
    }
}
