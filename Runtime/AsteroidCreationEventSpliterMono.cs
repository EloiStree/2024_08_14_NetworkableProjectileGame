using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidCreationEventSpliterMono : MonoBehaviour
{
    public SNAM16K_ObjectFloat m_asteroidRadiusSize;
    public SNAM16K_ObjectBool m_asteroidActive;

    public STRUCT_ProjectileCreationEvent m_lastReceived;
    public string m_lastReceivedString;
    public void PushIn(STRUCT_ProjectileCreationEvent receivedEvent) {

        m_lastReceived = receivedEvent;
        m_lastReceivedString= DateTime.UtcNow.ToString("HH:mm:ss.fff") + " " + receivedEvent.m_poolItemIndex + " " + receivedEvent.m_colliderRadius;
        if( m_asteroidRadiusSize)
        m_asteroidRadiusSize[receivedEvent.m_poolItemIndex]= receivedEvent.m_colliderRadius;
        if( m_asteroidActive)
        m_asteroidActive[receivedEvent.m_poolItemIndex] = true;
    }

    [ContextMenu("Set All Sized to One")]

    public void SetAllSizeToOne()
    {

        if (m_asteroidRadiusSize)
            for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
            {
                m_asteroidRadiusSize[i] = 1;
            }
    }
    [ContextMenu("Set Active All")]
    public void SetAllActive()
    {

        if (m_asteroidActive)
            for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
            {
                m_asteroidActive[i] = true;
            }
    }


}
