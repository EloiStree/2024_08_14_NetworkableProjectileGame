using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sleepy_SetBulletCountDifficulty : MonoBehaviour
{

    public UnityEvent<int> m_onCountChanged;

    public int m_minStartCount = 100;
    public int m_maxStartCount = 13000;

    public float m_maxSurvivalTime = 60 * 5;
    public AnimationCurve m_difficultyCurve;
    public StayAliveTimerCountMono m_timer;


    public int m_currentCount = 0;
    public int m_previouseCount = 0;

    public void Update()
    {
        float time = m_timer.m_timer;
        float timePercent = time / m_maxSurvivalTime;
        int count = Mathf.FloorToInt(Mathf.Lerp(m_minStartCount, m_maxStartCount, m_difficultyCurve.Evaluate(timePercent)));
        m_currentCount = count;
        if (m_currentCount != m_previouseCount)
        {
            m_previouseCount = m_currentCount;
            m_onCountChanged.Invoke(m_currentCount);
        }
        
    }

}
