using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagLayer : LayerBase
{
    public GameObject item;
    public Transform listContent;

    void Start()
    {
        for(int i = 0; i < GameData.getInstance().myBagsList.Count; i++)
        {
            GameObject obj = GameObject.Instantiate(item, listContent);

            PairData pairData = GameData.getInstance().myBagsList[i];
            obj.transform.Find("icon").GetComponent<Image>().sprite = CommonUtil.getSprite("Images/" + pairData.key);
            obj.transform.Find("count").GetComponent<Text>().text = pairData.value.ToString();
        }
    }
    
    void Update()
    {
        
    }

    public void onClickOk()
    {
        Destroy(gameObject);
    }
}
