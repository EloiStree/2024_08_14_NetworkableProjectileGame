using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNAM16KLogic_GeneralProjectileLifeTime : MonoBehaviour
{
    public float m_maxLifetime = 30f;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ProjectileCreatedEvent m_projectileCreatedEvent;

    public long m_timeOnServerUtcNowTicks;
    public bool m_useUpdateForServerTime = true;

    public void SetServerUtcTimeNow(long timeInUtcNowTicks)
    {
        m_timeOnServerUtcNowTicks = timeInUtcNowTicks;
    }

    public void DisableProjectileOutOfLifeTime() { 
        if (m_useUpdateForServerTime)
            SetServerUtcTimeNow(System.DateTime.UtcNow.Ticks);
        long tickLifeTime = ((long)m_maxLifetime) * System.TimeSpan.TicksPerSecond;

        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (m_isUsedProjectil[i])
            {
                if (m_timeOnServerUtcNowTicks-m_projectileCreatedEvent[i].m_serverUtcNowTicks > tickLifeTime)
                {
                    m_isUsedProjectil[i] = false;
                }
            }
        }
    

    }
}
