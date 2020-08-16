using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class JsonUtils
{
    static public List<T> loadJsonToList<T>(string jsonName)
    {
        StreamReader streamReader = new StreamReader(Application.dataPath + "/StreamingAssets/Datas/" + jsonName + ".json");
		string jsonData = streamReader.ReadToEnd();
		return JsonConvert.DeserializeObject<List<T>>(jsonData);
	}
}
