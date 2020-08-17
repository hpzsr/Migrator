using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsData
{
    public int id;
    public string name;
    public string hechengitem;
    public int maxinbag;
    public string cooktime;
    public string value;
}

public class MaterialsEntity
{
	static MaterialsEntity s_instance = null;

	public List<MaterialsData> dataList = new List<MaterialsData>();

	static public MaterialsEntity getInstance()
	{
		if (s_instance == null)
		{
			s_instance = new MaterialsEntity();
			s_instance.init();
		}

		return s_instance;
	}

	void init()
	{
		dataList = JsonUtils.loadJsonToList<MaterialsData>("materials");
	}

	public MaterialsData getDataById(int id)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			if (dataList[i].id == id)
			{
				return dataList[i];
			}
		}

		return null;
	}
}
