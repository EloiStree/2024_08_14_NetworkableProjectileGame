using UnityEngine;
using UnityEngine.Events;

public class SNAM16KEvent_RelayIndexToDestroyedEvent : MonoBehaviour
{
    public SNAM16K_ProjectileDestroyedEvent m_destroy;

    public UnityEvent<STRUCT_ProjectileDestructionEvent> m_onDestroyed;

    public STRUCT_ProjectileDestructionEvent m_lastDestroyed;

    public void PushInDestroyEvent(int index)
    {
        m_lastDestroyed = m_destroy[index];
        m_onDestroyed.Invoke(m_lastDestroyed);
    }
}
