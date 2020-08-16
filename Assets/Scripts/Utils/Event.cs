using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Event
{
    public delegate void EventCallBack(EventCallBackData data);
    static List<EventData> eventList = new List<EventData>();

    public class EventCallBackData
    {
        public int data_int = 0;
        public string data_string = "";
        public bool data_bool = true;
        public Transform data_transform = null;
    }

    class EventData
    {
        public string eventName = "";
        

        public EventCallBack callback;

        public EventData(string _eventName, EventCallBack _callback)
        {
            eventName = _eventName;
            callback = _callback;
        }
    }    

    public static void registerEvent(string eventName, EventCallBack callback)
    {
        EventData temp = getEventData(eventName);
        if(temp == null)
        {
            temp = new EventData(eventName, callback);
            eventList.Add(temp);
        }
        else
        {
            temp.callback = callback;
        }
    }

    public static void sendEvent(string eventName, EventCallBackData data)
    {
        EventData temp = getEventData(eventName);
        if (temp == null)
        {
            Debug.Log("没有此事件：" + eventName);
        }
        else
        {
            temp.callback(data);
        }
    }

    static EventData getEventData(string eventName)
    {
        for(int i = 0; i < eventList.Count; i++)
        {
            if(eventName == eventList[i].eventName)
            {
                return eventList[i];
            }
        }

        return null;
    }
}
