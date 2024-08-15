using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sleepy_StayAliveMono : MonoBehaviour
{
    public float m_timer;
    public Transform m_playerToMove;
    public Transform m_whereToRespawn;

    public string m_timerFormer="{0}:{1}";
    public UnityEvent<string> m_onTimingChanged;
    public UnityEvent<string> m_onPreviousChanged;
    public UnityEvent<string> m_onBestScoreChanged;

    public void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > 10)
        {
            m_playerToMove.position = m_whereToRespawn.position;
            m_timer = 0;
        }

        string result = ConvertToFormatFromTime(m_timer);
        m_onTimingChanged.Invoke(result);
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
    public void NotifyAsTouched()
    {
        m_previousScore = m_timer;
        m_onPreviousChanged.Invoke(ConvertToFormatFromTime(m_previousScore));
        if (m_timer > m_bestScore)
        {
            m_bestScore = m_timer;
            m_onBestScoreChanged.Invoke(ConvertToFormatFromTime(m_bestScore));
        }
        m_playerToMove.position = m_whereToRespawn.position;
    }
}
