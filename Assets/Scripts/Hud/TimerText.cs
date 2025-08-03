using System;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    private Text timerText;

    void Awake()
    {
        timerText = GetComponent<Text>();
    }

    public void SetText(float time)
    {
        if (time >= 1)
        {
            int timeInt = (int)time;
            timerText.text = timeInt.ToString();
        }
        else
        {
            timerText.text = time.ToString("F1");
        }
        if (time <= 0)
        {
            timerText.text = null;
        }
    }
}
