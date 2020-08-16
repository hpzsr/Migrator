using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogUtil : MonoBehaviour
{
    public static LogUtil s_instance = null;
    public Text text_log;

    void Start()
    {
        s_instance = this;
    }

    public void log(string str)
    {
        text_log.text += (str + "\n");
    }
}
