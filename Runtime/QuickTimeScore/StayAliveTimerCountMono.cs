using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StayAliveTimerCountMono : MonoBehaviour
{
    public float m_timer;

    public string m_timerFormer="{0:00}:{1:00}";
    public UnityEvent<float> m_onTimingChangedFloat;
    public UnityEvent<string> m_onTimingChanged;
    public UnityEvent<string> m_onPreviousChanged;
    public UnityEvent<string> m_onBestScoreChanged;

    public DateTime m_startTime;
    public void Start()
    {
        m_startTime = DateTime.Now;
    }
    public void Update()
    {
        m_timer = (float)(DateTime.Now- m_startTime).TotalSeconds;
        string result = ConvertToFormatFromTime(m_timer);
        m_onTimingChanged.Invoke(result);
        m_onTimingChangedFloat.Invoke(m_timer);
    }

    private string ConvertToFormatFromTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        string result = string.Format(m_timerFormer, minutes, seconds);
        return result;
    }

    public float m_previousScore;
    public float m_bestScore;
    public void ResetCountToZeroAndRecordScore()
    {
        m_previousScore = m_timer;
        m_onPreviousChanged.Invoke(ConvertToFormatFromTime(m_previousScore));
        if (m_timer > m_bestScore)
        {
            m_bestScore = m_timer;
            m_onBestScoreChanged.Invoke(ConvertToFormatFromTime(m_bestScore));
        }
        m_startTime = DateTime.Now;
        m_timer = 0;
    }
}


