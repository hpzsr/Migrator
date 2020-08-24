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

    List<PlayerSelfData> self_data = new List<PlayerSelfData>();

    ItemScript curLookPart = null;

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

        PlayerData playerData  = PlayerEntity.getInstance().getDataById(playerID);
        self_data.Add(new PlayerSelfData(playerID, 501, playerData.energy, playerData.energy));
        self_data.Add(new PlayerSelfData(playerID, 502, playerData.mind, playerData.mind));
        self_data.Add(new PlayerSelfData(playerID, 503, playerData.power, playerData.power));
    }

    public void changePlayerSelfData(Consts.PlayerInfo type,int value)
    {
        for(int i = 0; i < self_data.Count; i++)
        {
            if(self_data[i].type == (int)type)
            {
                self_data[i].curData += value;
                if(self_data[i].curData > self_data[i].fullData)
                {
                    self_data[i].curData = self_data[i].fullData;
                }
                else if (self_data[i].curData < 0)
                {
                    self_data[i].curData = 0;
                }
                break;
            }
        }

        if(FamilyLayer.s_instance.curControlPlayer != null)
        {
            FamilyLayer.s_instance.curControlPlayer.setInfo();
        }
    }

    public int getPlayerSelfDataByType(Consts.PlayerInfo type)
    {
        for(int i = 0; i < self_data.Count; i++)
        {
            if(self_data[i].type == (int)type)
            {
                return self_data[i].curData;
            }
        }

        return 0;
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
            playerInfo.Find("txt_501").GetComponent<Text>().text = getPlayerSelfDataByType(Consts.PlayerInfo.Energy) + "/" + playerData.energy;
            playerInfo.Find("txt_502").GetComponent<Text>().text = getPlayerSelfDataByType(Consts.PlayerInfo.Mind) + "/" + playerData.mind;
            playerInfo.Find("txt_503").GetComponent<Text>().text = getPlayerSelfDataByType(Consts.PlayerInfo.Power) + "/" + playerData.power;
        }
    }

    // 移动到指定地点
    public void moveTo(Consts.MoveEndEvent moveEndEvent, Vector2 targetPos,Transform targetTrans = null)
    {
        if (targetTrans)
        {
            curLookPart = targetTrans.GetComponent<ItemScript>();
        }
        
        if (move_seq != null)
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
                if (curLookPart.isCanUpgrade && moveEndEvent == Consts.MoveEndEvent.Upgrade)
                {
                    var data = new Event.EventCallBackData();
                    data.data_transform = targetTrans;
                    LayerManager.showLayer(Consts.Layer.BuildLayer, data);
                }
                else if (moveEndEvent == Consts.MoveEndEvent.Find)
                {
                    var data = new Event.EventCallBackData();
                    data.data_transform = targetTrans;
                    data.data_bool = true;
                    data.data_string = playerID + "";
                    LayerManager.showLayer(Consts.Layer.FindLayer, data);
                }
                else if (moveEndEvent == Consts.MoveEndEvent.Make)
                {
                    if (curLookPart.level > 0)
                    {
                        var data = new Event.EventCallBackData();
                        data.data_transform = targetTrans;
                        LayerManager.showLayer(Consts.Layer.MakeLayer, data);
                    }
                    else
                    {
                        var data = new Event.EventCallBackData();
                        data.data_transform = targetTrans;
                        LayerManager.showLayer(Consts.Layer.BuildLayer, data);
                    }
                }
                else if (moveEndEvent == Consts.MoveEndEvent.Toy)
                {
                    if (FamilyLayer.s_instance.trans_workbench.GetComponent<ItemScript>().level > 0)
                    {
                        if (FamilyLayer.s_instance.trans_toy.GetComponent<ItemScript>().level == 0)
                        {
                            var data = new Event.EventCallBackData();
                            data.data_transform = targetTrans;
                            LayerManager.showLayer(Consts.Layer.MakeLayer, data);
                        }
                        else
                        {
                        }
                    }
                }
                else if (moveEndEvent == Consts.MoveEndEvent.Vase)
                {
                    if (FamilyLayer.s_instance.trans_workbench.GetComponent<ItemScript>().level > 0)
                    {
                        if (FamilyLayer.s_instance.trans_vase.GetComponent<ItemScript>().level == 0)
                        {
                            var data = new Event.EventCallBackData();
                            data.data_transform = targetTrans;
                            LayerManager.showLayer(Consts.Layer.MakeLayer, data);
                        }
                        else
                        {
                        }
                    }
                }
                else if (moveEndEvent == Consts.MoveEndEvent.Gitar)
                {
                    if (FamilyLayer.s_instance.trans_workbench.GetComponent<ItemScript>().level > 0)
                    {
                        if (FamilyLayer.s_instance.trans_gitar.GetComponent<ItemScript>().level == 0)
                        {
                            var data = new Event.EventCallBackData();
                            data.data_transform = targetTrans;
                            LayerManager.showLayer(Consts.Layer.MakeLayer, data);
                        }
                        else
                        {
                        }
                    }
                }
                else if (moveEndEvent >= Consts.MoveEndEvent.Food_209)
                {
                    GameData.getInstance().changeBagItemCount((int)moveEndEvent, -1);
                    targetTrans.GetComponent<ItemScript>().setCanEatFoods();

                    MaterialsData materialsData = MaterialsEntity.getInstance().getDataById((int)moveEndEvent);
                    string[] buff_array = materialsData.value.Split(';');
                    for (int i = 0; i < buff_array.Length; i++)
                    {
                        int type = int.Parse(buff_array[i].Split(':')[0]);
                        int value = int.Parse(buff_array[i].Split(':')[1]);
                        changePlayerSelfData((Consts.PlayerInfo)type, value);
                    }
                    ToastScript.show("Eat " + materialsData.name);
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
