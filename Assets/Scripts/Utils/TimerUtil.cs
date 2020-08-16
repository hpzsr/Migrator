using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TimerUtil : MonoBehaviour
{
    public static TimerUtil s_instance = null;

    public delegate void OnCallBack();

    List<TimerData> timerDataList = new List<TimerData>();

    class TimerData
    {
        public float curTime = 0;

        public float endTime;
        public OnCallBack onCallBack;

        public TimerData(float _endTime, OnCallBack _onCallBack)
        {
            endTime = _endTime;
            onCallBack = _onCallBack;
        }
    }

    public static TimerUtil getInstance()
    {
        if(s_instance == null)
        {
            GameObject obj = new GameObject();
            s_instance = obj.AddComponent<TimerUtil>();
            obj.name = "TimerUtil";
            DontDestroyOnLoad(obj);
        }

        return s_instance;
    }

    public void delayTime(float timeSeconds,OnCallBack onCallBack)
    {
        timerDataList.Add(new TimerData(timeSeconds, onCallBack));
    }    
    
    void Update()
    {
        for(int i = 0; i < timerDataList.Count; i++)
        {
            timerDataList[i].curTime += Time.deltaTime;
        }

        for (int i = timerDataList.Count - 1; i >= 0 ; i--)
        {
            if(timerDataList[i].curTime >= timerDataList[i].endTime)
            {
                if (timerDataList[i].onCallBack != null)
                {
                    timerDataList[i].onCallBack();
                }
                timerDataList.RemoveAt(i);
            }
        }
    }
}
