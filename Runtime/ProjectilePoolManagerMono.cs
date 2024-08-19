using System;
using System.Collections.Generic;
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
   

    public string m_lastSpawn;
    public int m_maxAttemptAtFindingClaim=500;

    public int m_lastCursorIndex = 0;
    public void UnspawnFromIndex(int index) { 
    
        m_isUsedProjectil[index] = false;

    }


    public int m_unclaimedQueueCount = 1000;
    public Queue<int> m_mayBeUnclaimed= new Queue<int>();
    public int m_queueCountState;

    public void EnqueueUnclaimedIndex(int index) {

        if (m_mayBeUnclaimed.Count < m_unclaimedQueueCount) { 
            m_mayBeUnclaimed.Enqueue(index);
            m_queueCountState= m_mayBeUnclaimed.Count;
        }
    }



    public bool m_allowRandomOverride = true;
    public void Spawn(Vector3 position, Vector3 forward, Quaternion rotation, float projectileSpeed, float projectileRadius)
    {
        bool isClaimable = false;
        int index = -1;

        while (m_mayBeUnclaimed.Count > 0) {
            index = m_mayBeUnclaimed.Dequeue();
            if (!m_isUsedProjectil[index]) { 
                isClaimable= true;
                break;
            }
        }
        m_queueCountState = m_mayBeUnclaimed.Count;
        if (!isClaimable) { 
            GetClaimableProjectil(ref m_lastCursorIndex, out  isClaimable, out  index, m_maxAttemptAtFindingClaim);
        }

        if (!isClaimable && m_allowRandomOverride) { 
            isClaimable = true;
             index= UnityEngine.Random.Range(0, m_isUsedProjectil.GetLength());
    
        }
        
        if (isClaimable && index>=0 && index< SNAM16K.ARRAY_MAX_SIZE)
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
            //m_lastSpawn = now.ToString("HH:mm:ss.fff");
            
        }
    }

    public void GetClaimableProjectil(out bool isClaimable, out int index)
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
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
    public void GetClaimableProjectil(ref int startIndex, out bool isClaimable, out int index)
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            int modIndex = (i + startIndex) % SNAM16K.ARRAY_MAX_SIZE;
            if (!m_isUsedProjectil[modIndex])
            {
                index = modIndex;
                isClaimable = true;
                startIndex = modIndex;
                return;
            }
        }

        isClaimable = false;
        index = -1;
    }
    public void GetClaimableProjectil(ref int startIndex, out bool isClaimable, out int index, int maxBatch)
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            int modIndex = (i + startIndex) % SNAM16K.ARRAY_MAX_SIZE;
            if (!m_isUsedProjectil[modIndex])
            {
                index = modIndex;
                isClaimable = true;
                startIndex = modIndex;
                return;
            }
        }

        isClaimable = false;
        index = -1;
    }
}

