using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public static float DevScreenWidth = 1920;
    public static float DevScreenHeight = 1080;

    public static float ScreenWidth = 1920;
    public static float ScreenHeight = 1080;

    public static float getWidth()
    {
        // 如果设备比设计长，则使用设备宽度
        if(((float)Screen.width / (float)Screen.height) >= (DevScreenWidth / DevScreenHeight))
        {
            return Screen.width;
        }

        return DevScreenWidth;
    }

    public static float getHeight()
    {
        return DevScreenHeight;

        //// 如果设备比设计长，则使用设备高度
        //if ((float)Screen.width / (float)Screen.height <= DevScreenWidth / DevScreenHeight)
        //{
        //    return DevScreenHeight;
        //}

        //return (DevScreenWidth / DevScreenHeight) / ((float)Screen.width / (float)Screen.height) * Screen.height * 2;
    }

    public enum Layer
    {
        FamilyLayer,
        BuildLayer,
        FindLayer,
        BagLayer,
        ReadyFindLayer,
        MapLayer,
        NightLayer,
        ReadyNightLayer,
    }
}
