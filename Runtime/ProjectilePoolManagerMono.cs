using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ProjectilePoolManagerMono : MonoBehaviour {

    public byte m_poolId = 71;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ObjectVector3 m_projectilPosition;
    public SNAM16K_ProjectileCapsulePosition m_projectilDirection;
    public SNAM16K_ProjectileMoveConstant m_projectilSpeed;
    public SNAM16K_ProjectileCreatedEvent m_createdEvent;
    [Header("Events")]
    public bool m_useUnityEvent;
    public UnityEvent<STRUCT_ProjectileCreationEvent> m_onCreatedEvent;
    public Action<STRUCT_ProjectileCreationEvent> m_onCreatedAction;

    public string m_lastSpawn;


    public void UnspawnFromIndex(int index) { 
    
        m_isUsedProjectil[index] = false;

    }

    public void Spawn(Vector3 position, Vector3 forward, Quaternion rotation, float projectileSpeed, float projectileRadius)
    {
        GetClaimableProjectil(out bool isClaimable, out int index);
        if (isClaimable)
        {
            DateTime now = DateTime.UtcNow;
            STRUCT_ProjectileCreationEvent creationEvent = new STRUCT_ProjectileCreationEvent();
            STRUCT_ProjectileCapsulePosition capsulePosition = new STRUCT_ProjectileCapsulePosition();
            STRUCT_ProjectileMoveConstant moveConstant = new STRUCT_ProjectileMoveConstant();

            capsulePosition.m_capsuleRadius = projectileRadius;
            capsulePosition.m_currentPosition = position;
            capsulePosition.m_previousPosition = position;

            moveConstant.m_startPoint = position;
            moveConstant.m_startUtcNowTicks= now.Ticks;
            moveConstant.m_speedInMetersPerSecond = projectileSpeed;
            moveConstant.m_direction = forward;

            
            creationEvent.m_startPosition = position;
            creationEvent.m_startDirection = forward;
            creationEvent.m_startRotation = rotation;
            creationEvent.m_speedInMetersPerSecond = projectileSpeed;
            creationEvent.m_colliderRadius = projectileRadius;
            creationEvent.m_poolId = m_poolId;
            creationEvent.m_poolItemIndex = index;
            creationEvent.m_serverUtcNowTicks = now.Ticks;
            m_projectilPosition[index] = position;
            m_createdEvent[index] = creationEvent;
            m_isUsedProjectil[index] = true;
            m_projectilDirection[index]= capsulePosition;
            m_projectilSpeed[index] = moveConstant;
            m_lastSpawn = now.ToString("HH:mm:ss.fff");
            if (m_useUnityEvent) { 
                m_onCreatedEvent.Invoke(creationEvent);
            }
            if(m_onCreatedAction != null)
            {
                m_onCreatedAction(creationEvent);
            }
        }
    }

    public void GetClaimableProjectil(out bool isClaimable, out int index)
    {
        for (int i = 0; i < m_isUsedProjectil.GetLength(); i++)
        {
            if (!m_isUsedProjectil[i])
            {
                index = i;
                isClaimable = true;
                return;
            }
        }
        isClaimable = false;
        index = -1;
    }
}
public class SNAM16KLogic_BasicLifeTimeProjectiles : MonoBehaviour
{
    float m_maxLifeTime = 60.0f;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ObjectBool m_hadBeenDestroy;
    public SNAM16K_ProjectileDestroyedEvent m_destroyEventHolder;

    public void Apply() { 
    


    }

}
