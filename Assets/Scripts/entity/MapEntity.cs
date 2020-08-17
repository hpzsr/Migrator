using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public int map;
    public int point;
    public string Reward;
    public int energy;
    public int floor;
    public int nextFloorPoint;
}

public class MapEntity
{
	static MapEntity s_instance = null;

	public List<MapData> materialsList = new List<MapData>();

	static public MapEntity getInstance()
	{
		if(s_instance == null)
		{
			s_instance = new MapEntity();
			s_instance.init();
		}

		return s_instance;
	}

	void init()
	{
        materialsList = JsonUtils.loadJsonToList<MapData>("map");
	}

	public MapData getDataByMap(int map,int point)
	{
		for(int i = 0; i < materialsList.Count; i++)
		{
			if((materialsList[i].map == map) && (materialsList[i].point == point))
			{
				return materialsList[i];
			}
		}

		return null;
	}

    public List<MapData> getDataByMapFloor(int map, int floor)
    {
        List<MapData> list = new List<MapData>();
        for (int i = 0; i < materialsList.Count; i++)
        {
            if ((materialsList[i].map == map) && (materialsList[i].floor == floor))
            {
				list.Add(materialsList[i]);
            }
        }

        return list;
    }
}
