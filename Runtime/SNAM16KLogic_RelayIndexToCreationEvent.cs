using UnityEngine;
using UnityEngine.Events;

public class SNAM16KLogic_RelayIndexToCreationEvent : MonoBehaviour
{
    public SNAM16K_ProjectileCreatedEvent m_created;
    public SNAM16K_ProjectileDestroyedEvent m_destroy;

    public UnityEvent<STRUCT_ProjectileCreationEvent> m_onCreated;
    public UnityEvent<STRUCT_ProjectileDestructionEvent> m_onDestroyed;

    public STRUCT_ProjectileCreationEvent m_lastCreated;
    public STRUCT_ProjectileDestructionEvent m_lastDestroyed;

    public void PushInCreatedEvent(int index)
    {
        m_lastCreated = m_created[index];
        m_onCreated.Invoke(m_lastCreated);
    }
    public void PushInDestroyEvent(int index)
    {
        m_lastDestroyed = m_destroy[index];
        m_onDestroyed.Invoke(m_lastDestroyed);
    }
}
