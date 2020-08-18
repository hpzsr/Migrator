using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int id;
    public string name;
    public int energy;
    public int mind;
    public int power;
	public int bagCount;
}

public class PlayerEntity
{
	static PlayerEntity s_instance = null;

	public List<PlayerData> materialsList = new List<PlayerData>();

	static public PlayerEntity getInstance()
	{
		if(s_instance == null)
		{
			s_instance = new PlayerEntity();
			s_instance.init();
		}

		return s_instance;
	}

	void init()
	{
        materialsList = JsonUtils.loadJsonToList<PlayerData>("player");
	}

	public PlayerData getDataById(int id)
	{
		for(int i = 0; i < materialsList.Count; i++)
		{
			if(materialsList[i].id == id)
			{
				return materialsList[i];
			}
		}

		return null;
	}
}
