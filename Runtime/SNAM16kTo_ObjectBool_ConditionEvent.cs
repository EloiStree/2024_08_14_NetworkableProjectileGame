using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SNAM16kTo_ObjectBool_ConditionEvent : MonoBehaviour
{
    
    public SNAM16K_ObjectBool m_condition;
    public UnityEvent m_eventTrue;
    public UnityEvent m_eventFalse;
    public bool m_useEventTrue;
    public UnityEvent<int> m_onTrue;
    public bool m_useEventFalse;
    public UnityEvent<int> m_onFalse;

    public bool m_hasTrue;
    public bool m_hasFalse;

    public void Update()
    {
        bool hasTrue = false;
        bool hasFalse = false;
        for (int i = 0; i < m_condition.GetNativeArray().Length; i++)
        {
            bool value = m_condition.GetNativeArray()[i];
            if (!hasTrue &&  value)
            {
                hasTrue = true;
            }
            if(!hasFalse && !value)
            {
                hasFalse = true;
            }
            if(value)
                if(m_useEventTrue)
                    m_onTrue.Invoke(i);
            else
                if(m_useEventFalse)
                    m_onFalse.Invoke(i);

        }
        m_hasFalse = hasFalse;
        m_hasTrue = hasTrue;
       
    }
}
