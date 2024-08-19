using UnityEngine;
using UnityEngine.Events;


public class SNAM16KEvent_RelayIndexToCreatedEvent : MonoBehaviour
{
    public SNAM16K_ProjectileCreatedEvent m_created;

    public UnityEvent<STRUCT_ProjectileCreationEvent> m_onCreated;

    public STRUCT_ProjectileCreationEvent m_lastCreated;

    public void PushInCreatedEvent(int index)
    {
        m_lastCreated = m_created[index];
        m_onCreated.Invoke(m_lastCreated);
    }
}
