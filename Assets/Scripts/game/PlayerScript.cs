using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public Image self_img;
    public int playerID = 0;
    public int floor;

    Image img_choice;
    Sequence move_seq = null;
    Vector2 beforePostion;

    void Start()
    {
        img_choice = transform.Find("choice").GetComponent<Image>();
    }
    
    void Update()
    {
        if(transform.localPosition.x > beforePostion.x)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        else if (transform.localPosition.x < beforePostion.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        beforePostion = transform.localPosition;
    }

    public void init()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(floor), 0);
        beforePostion = transform.localPosition;
    }

    // 设置该人物是否被选中、控制
    public void setIsChoice(bool isChoice)
    {
        if(isChoice)
        {
            img_choice.transform.localScale = new Vector3(1,1,1);
            Image head = FamilyLayer.s_instance.playerInfo.Find("head").GetComponent<Image>();
            head.sprite = CommonUtil.getSprite("images/head" + playerID);
            head.SetNativeSize();
            setInfo();
        }
        else
        {
            img_choice.transform.localScale = new Vector3(0,0,0);
        }
    }

    public void setInfo()
    {
        Transform playerInfo = FamilyLayer.s_instance.playerInfo;

        PlayerData playerData  = PlayerEntity.getInstance().getDataById(playerID);
        if (playerData != null)
        {
            playerInfo.Find("name").GetComponent<Text>().text = playerData.name;
            playerInfo.Find("txt_501").GetComponent<Text>().text = "0/" + playerData.energy;
            playerInfo.Find("txt_502").GetComponent<Text>().text = "0/" + playerData.mind;
            playerInfo.Find("txt_503").GetComponent<Text>().text = "0/" + playerData.power;
        }
    }

    // 移动到指定地点
    public void moveTo(bool isUpgrade,Vector2 targetPos,Transform targetTrans = null)
    {
        if(move_seq != null)
        {
            move_seq.Kill();
        }

        List<Vector2> list_passPoint = new List<Vector2>();

        int curFloor = getInWhichFloor(transform.localPosition);
        int targetFloor = getInWhichFloor(targetPos);
        if(curFloor == targetFloor)
        {
            list_passPoint.Add(new Vector2(transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor)));
            list_passPoint.Add(new Vector2(targetPos.x, FamilyLayer.s_instance.getFloorPosY(targetFloor)));
        }
        else if (Mathf.Abs(curFloor - targetFloor) == 1)
        {
            Transform ladder = findLadder(transform, getInWhichFloor(transform.localPosition),targetFloor);

            if ((int)transform.localPosition.y == (int)FamilyLayer.s_instance.getFloorPosY(curFloor))
            {
                list_passPoint.Add(new Vector2(ladder.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor)));
            }
            list_passPoint.Add(new Vector2(ladder.transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(targetFloor)));
            list_passPoint.Add(new Vector2(targetPos.x, FamilyLayer.s_instance.getFloorPosY(targetFloor)));
        }
        else if (Mathf.Abs(curFloor - targetFloor) == 2)
        {
            Transform ladder1 = null;
            Transform ladder2 = null;
            // 上楼
            if (curFloor < targetFloor)
            {
                ladder1 = findLadder(transform, getInWhichFloor(transform.localPosition), curFloor + 1);
                ladder2 = findLadder(ladder1, curFloor + 1, curFloor + 2);
            }
            // 下楼
            else
            {
                ladder1 = findLadder(transform, getInWhichFloor(transform.localPosition), curFloor - 1);
                ladder2 = findLadder(ladder1, curFloor - 1, curFloor - 2);
            }

            if ((int)transform.localPosition.y == (int)FamilyLayer.s_instance.getFloorPosY(curFloor))
            {
                list_passPoint.Add(new Vector2(ladder1.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor)));
            }
            list_passPoint.Add(new Vector2(ladder1.transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor < targetFloor ? curFloor + 1 : curFloor - 1)));
            list_passPoint.Add(new Vector2(ladder2.transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor < targetFloor ? curFloor + 1 : curFloor - 1)));
            list_passPoint.Add(new Vector2(ladder2.transform.localPosition.x, FamilyLayer.s_instance.getFloorPosY(curFloor < targetFloor ? curFloor + 2 : curFloor - 2)));
            list_passPoint.Add(new Vector2(targetPos.x, FamilyLayer.s_instance.getFloorPosY(curFloor < targetFloor ? curFloor + 2 : curFloor - 2)));
        }

        move_seq = DOTween.Sequence();
        for (int i = 0; i < list_passPoint.Count; i++)
        {
            float time = 0;
            if (i == 0)
            {
                time = getMoveTime(CommonUtil.twoObjDistance_2D(transform.localPosition, list_passPoint[i]));
            }
            else
            {
                time = getMoveTime(CommonUtil.twoObjDistance_2D(list_passPoint[i - 1], list_passPoint[i]));
            }
            move_seq.Append(self_img.rectTransform.DOAnchorPos(list_passPoint[i], time).SetEase(Ease.Linear));
        }
        move_seq.Play().OnComplete(()=> {
            if(targetTrans != null)
            {
                if (targetTrans.GetComponent<ItemScript>().isCanUpgrade && isUpgrade)
                {
                    var data = new Event.EventCallBackData();
                    data.data_transform = targetTrans;
                    LayerManager.showLayer(Consts.Layer.BuildLayer, data);
                }
                else if(targetTrans.tag == "find")
                {
                    var data = new Event.EventCallBackData();
                    data.data_transform = targetTrans;
                    data.data_bool = true;
                    LayerManager.showLayer(Consts.Layer.FindLayer, data);
                }
            }
        });
    }

    // 判断当前在几楼
    public int getInWhichFloor(Vector2 targetPos)
    {
        if (targetPos.y >= FamilyLayer.s_instance.floor3.transform.localPosition.y)
        {
            return 3;
        }
        else if (targetPos.y < FamilyLayer.s_instance.floor2.transform.localPosition.y)
        {
            return 1;
        }

        return 2;
    }

    // 寻找梯子
    public Transform findLadder(Transform trans,int curFloor,int targetFloor)
    {
        if(targetFloor == curFloor)
        {
            return null;
        }

        List<Transform> list = new List<Transform>();
        for(int i = 0; i < FamilyLayer.s_instance.list_ladder.Count; i++)
        {
            Transform ladder = FamilyLayer.s_instance.list_ladder[i];
            int ladderFloor = getInWhichFloor(ladder.localPosition);

            if(targetFloor > curFloor)
            {
               if(curFloor == ladderFloor)
               {
                    list.Add(ladder);
               }
            }
            else if (targetFloor < curFloor)
            {
                if ((curFloor -ladderFloor) == 1)
                {
                    list.Add(ladder);
                }
            }
        }

        if (list.Count > 0)
        {
            Transform choice = list[0];
            float juli_x = Mathf.Abs(trans.transform.localPosition.x - choice.transform.localPosition.x);
            for(int i = 1; i < list.Count; i++)
            {
                float temp = Mathf.Abs(list[i].transform.localPosition.x - trans.transform.localPosition.x);
                if(temp < juli_x)
                {
                    choice = list[i];
                    juli_x = temp;
                }
            }

            return choice;
        }

        return null;
    }

    // 根据距离计算移动时间，保持匀速
    public float getMoveTime(float juli)
    {
        return Mathf.Abs(juli * 0.005f) * 0.8f;
    }
}
