using Eloi.WatchAndDate;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using DroneIMMO;

public class AsteroideJobManagerMono : MonoBehaviour
{

    public byte m_poolId=1;
    public int m_poolItemMax = 128*128;
    public int m_numberOfAsteroidsInGame = 20;
    public SNAM16K_ObjectBool m_isExisting;
    public SNAM16K_ObjectBool m_asteroidDestroyedEvent;
    public SNAM16K_ObjectBool m_asteroidCreationEvent;
    public SNAM16K_ProjectileCreatedEvent m_asteroidInGame;
    public SNAM16K_ProjectileMoveConstant m_asteroidMoveUpdateInfo;
    public SNAM16K_ProjectileCapsulePosition  m_asteroidPosition;
    public SNAM16K_ObjectVector3 m_currentPosition;


    public Transform m_centerOfSpace;

    public float m_skyHeight=ushort.MaxValue/2f;
    public float m_squareWidth = ushort.MaxValue / 2f;

    public UnityEvent<STRUCT_ProjectileCreationEvent> m_onAsteroidCreated;
    public UnityEvent<STRUCT_ProjectileDestructionEvent> m_onAsteroidDestroyed;

    public float m_minSpeed=1;
    public float m_maxSpeed=10;
    public float m_minSize = 0.1f;
    public float m_maxSize = 1f;

    public void OnEnable()
    {
        m_poolItemMax = SNAM16K.ARRAY_MAX_SIZE;
        RandomizedAll();
    }

    [ContextMenu("Randomized All")]
    public void RandomizedAll()
    {
        for (int i = 0; i < m_poolItemMax; i++)
        {
            STRUCT_ProjectileCreationEvent a = m_asteroidInGame[i];
            STRUCT_ProjectileMoveConstant m= m_asteroidMoveUpdateInfo[i];
            a.m_poolId = m_poolId;
            a.m_poolItemIndex = i;
            SetRandomStartPointTo(ref a,ref m);
            m_asteroidInGame[i] = a;
            m_asteroidMoveUpdateInfo[i] = m;
            m_onAsteroidCreated.Invoke(a);
            if(m_onAsteroidChangedIndex!=null)
                m_onAsteroidChangedIndex.Invoke(i);
        }
    }

    private void SetRandomStartPointTo(ref STRUCT_ProjectileCreationEvent asteroidCreationEvent, ref STRUCT_ProjectileMoveConstant moveInfo)
    {
        asteroidCreationEvent.m_startPosition = new Vector3(UnityEngine.Random.Range(-m_squareWidth, m_squareWidth), UnityEngine.Random.Range(0, m_skyHeight), UnityEngine.Random.Range(-m_squareWidth, m_squareWidth));
        asteroidCreationEvent.m_startRotation = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        asteroidCreationEvent.m_startDirection = asteroidCreationEvent.m_startRotation * Vector3.forward;
        asteroidCreationEvent.m_speedInMetersPerSecond = UnityEngine.Random.Range(m_minSpeed, m_maxSpeed);
        asteroidCreationEvent.m_colliderRadius = UnityEngine.Random.Range(m_minSize, m_maxSize);
        asteroidCreationEvent.m_serverUtcNowTicks = DateTime.UtcNow.Ticks;


        moveInfo.m_startUtcNowTicks = asteroidCreationEvent.m_serverUtcNowTicks;
        moveInfo.m_speedInMetersPerSecond = asteroidCreationEvent.m_speedInMetersPerSecond;
        moveInfo.m_startPoint = asteroidCreationEvent.m_startPosition;
        moveInfo.m_direction = asteroidCreationEvent.m_startDirection;



    }

    public long m_currentTickServerUtcNow;
    public long m_currentTickServerUtcPrevious;
    public long m_updateTick;
    public WatchAndDateTimeActionResult m_moveObject;
    public WatchAndDateTimeActionResult m_outOfBoxed;
    public WatchAndDateTimeActionResult m_broadcastNewPosition;

    private void Awake()
    {
        SetProjectileInGameToInsepctorValue();
    }

    private void SetProjectileInGameToInsepctorValue()
    {
        SetProjectileInGame(m_numberOfAsteroidsInGame);
    }

    public void SetProjectileInGame(int projectile)
    {
        for (int i = 0; i < m_isExisting.GetLength(); i++)
        {
            m_isExisting[i] = i < projectile;
        }
    }

    public void Update()
    {
        m_moveObject.StartCounting();
        m_updateTick++;
        m_currentTickServerUtcPrevious = m_currentTickServerUtcNow;
        m_currentTickServerUtcNow = DateTime.UtcNow.Ticks;
        STRUCTJOB_ProjectileMoveJob moveJob = new STRUCTJOB_ProjectileMoveJob();
        moveJob.m_projectileInGame = m_asteroidMoveUpdateInfo.GetNativeArray();
        moveJob.m_currentExistance = m_asteroidPosition.GetNativeArray();
        moveJob.m_isUsed = m_isExisting.GetNativeArray();
        moveJob.m_currentMaxAsteroide = m_numberOfAsteroidsInGame;
        moveJob.m_serverCurrentUtcNowTicks = m_currentTickServerUtcNow;
        moveJob.m_serverCurrentUtcPreviousTicks = m_currentTickServerUtcPrevious;
        moveJob.m_currentPosition = m_currentPosition.GetNativeArray();
        moveJob.m_hidePosition = new Vector3(-404, -404, -404);
        JobHandle moveJobHandle = moveJob.Schedule(m_numberOfAsteroidsInGame, 64);
        moveJobHandle.Complete();

       m_moveObject.StopCounting();
        m_outOfBoxed.StartCounting();
        STRUCTJOB_AsteroideOutOfBoundJob outOfBoundJob = new STRUCTJOB_AsteroideOutOfBoundJob();
        outOfBoundJob.m_destroyEvent = m_asteroidDestroyedEvent.GetNativeArray();
        outOfBoundJob.m_currentExistance = m_asteroidPosition.GetNativeArray();
        outOfBoundJob.m_currentMaxAsteroide = m_numberOfAsteroidsInGame;
        outOfBoundJob.m_centerPosition = m_centerOfSpace.position;
        outOfBoundJob.m_maxHeightDistance = m_skyHeight;
        outOfBoundJob.m_maxWidthDistance = m_squareWidth;
        JobHandle outOfBoundJobHandle = outOfBoundJob.Schedule(m_numberOfAsteroidsInGame, 64);
        outOfBoundJobHandle.Complete();

        m_outOfBoxed.StopCounting();

        m_broadcastNewPosition.StartCounting();
        for (int i = 0; i < m_numberOfAsteroidsInGame; i++)
        {
            if (m_asteroidDestroyedEvent[i])
            {
                m_asteroidDestroyedEvent[i] = false;
                m_asteroidCreationEvent[i] = true;
                STRUCT_ProjectileCreationEvent a = m_asteroidInGame[i];
                STRUCT_ProjectileMoveConstant m = m_asteroidMoveUpdateInfo[i];
                SetRandomStartPointTo(ref a, ref m);

                m_asteroidMoveUpdateInfo[i] = m;
                m_asteroidInGame[i] = a;
                m_onAsteroidDestroyed.Invoke(new STRUCT_ProjectileDestructionEvent() { 
                    m_poolId = m_poolId, 
                    m_poolItemIndex = i,
                    m_serverUtcNowTicks = 
                    m_currentTickServerUtcNow });
            }
        }
        for (int i = 0; i < m_numberOfAsteroidsInGame; i++) { 
            if (m_asteroidCreationEvent[i])
            {
                m_asteroidCreationEvent[i] = false;
                m_onAsteroidCreated.Invoke(m_asteroidInGame[i]);
                if(m_onAsteroidChangedIndex!=null)  
                    m_onAsteroidChangedIndex.Invoke(i);
            }
        }
        if (m_debugLine) { 
            for(int i = 0;  i < m_numberOfAsteroidsInGame; i++)
            {
                Debug.DrawLine(m_asteroidPosition[i].m_currentPosition, m_asteroidPosition[i].m_previousPosition, Color.red);
            }
        }
        m_broadcastNewPosition.StopCounting();


    }
    public bool m_debugLine=true;
    


    public Action<int> m_onAsteroidChangedIndex;

    public void AddCreationNewAsteroidIndex(Action<int> onAsteroidchanged)
    {
        m_onAsteroidChangedIndex += onAsteroidchanged;
    }
    public void RemoveCreationnewAsteroidIndex(Action<int> onAsteroidchanged)
    {
        m_onAsteroidChangedIndex -= onAsteroidchanged;
    }
}




[System.Serializable]
public struct STRUCT_ProjectileCreationEvent {
    public byte m_poolId;
    public int m_poolItemIndex;
    public long m_serverUtcNowTicks;
    public Vector3 m_startPosition;
    public Quaternion m_startRotation;
    public Vector3 m_startDirection;
    public float m_speedInMetersPerSecond;
    public float m_colliderRadius;
}

[System.Serializable]
public struct STRUCT_ProjectileMoveConstant {

    public long m_startUtcNowTicks;
    public float m_speedInMetersPerSecond;
    public Vector3 m_startPoint;
    public Vector3 m_direction;
}



[System.Serializable]
public struct STRUCT_ProjectileDestructionEvent { 
    public byte m_poolId;
    public int m_poolItemIndex;
    public long m_serverUtcNowTicks;
}

[System.Serializable]
public struct STRUCT_ProjectileCapsulePosition
{
    public Vector3 m_currentPosition;
    public Vector3 m_previousPosition;
    public float m_capsuleRadius;
}

[BurstCompile]
public struct STRUCTJOB_ProjectileMoveJob : IJobParallelFor
{

    [ReadOnly]
    public NativeArray<bool> m_isUsed;
    [ReadOnly]
    public NativeArray<STRUCT_ProjectileMoveConstant> m_projectileInGame;

    public NativeArray<STRUCT_ProjectileCapsulePosition> m_currentExistance;
    [WriteOnly]
    public NativeArray<Vector3> m_currentPosition;
    public int m_currentMaxAsteroide;
    public long m_serverCurrentUtcNowTicks;
    public long m_serverCurrentUtcPreviousTicks;
    public Vector3 m_hidePosition;
    public const float m_tickToSeconds= 1f/(float)TimeSpan.TicksPerSecond;

    public void Execute(int index)
    {
        if (index >= m_currentMaxAsteroide)
        {

            return;
        }
        if (!m_isUsed[index])
        {
            m_currentPosition[index] = m_hidePosition;

            return;
            
        }
        STRUCT_ProjectileMoveConstant a = m_projectileInGame[index];
        STRUCT_ProjectileCapsulePosition p = m_currentExistance[index];
     
        p.m_previousPosition = p.m_currentPosition;
        float timeSinceStart = 
            (m_serverCurrentUtcNowTicks - m_projectileInGame[index].m_startUtcNowTicks)*m_tickToSeconds;
        float distance = m_projectileInGame[index].m_speedInMetersPerSecond * timeSinceStart;
        p.m_currentPosition= a.m_startPoint+ a.m_direction * distance;
        m_currentExistance[index] = p;
        m_currentPosition[index]= p.m_currentPosition;
    }
}


public struct STRUCTJOB_AsteroideMoveApplyToTransform : IJobParallelForTransform
{
    public NativeArray<STRUCT_ProjectileCapsulePosition> m_currentExistance;
    public int m_currentMaxAsteroide;
    public Vector3 m_unusedWorldPosition;

    public void Execute(int index, TransformAccess transform)
    {
        if (index >= m_currentMaxAsteroide)
        {
            transform.position = m_unusedWorldPosition;
            return;
        }
        transform.position = m_currentExistance[index].m_currentPosition;
    }
}


public struct STRUCTJOB_AsteroideOutOfBoundJob : IJobParallelFor
{
    public NativeArray<bool> m_destroyEvent;
    public NativeArray<STRUCT_ProjectileCapsulePosition> m_currentExistance;
    public long m_serverCurrentUtcNowTicks;
    public long m_serverCurrentUtcPreviousTicks;

    public int m_currentMaxAsteroide;
    public Vector3 m_centerPosition;
    public float m_maxHeightDistance;
    public float m_maxWidthDistance;
    public bool m_useDownPart;

    public void Execute(int index)
    {
        m_destroyEvent[index] = false;
        if (index>=m_currentMaxAsteroide)
        {
            return;
        }
       
        if (m_useDownPart)
        {
            if (m_currentExistance[index].m_currentPosition.y > m_maxHeightDistance
                        || m_currentExistance[index].m_currentPosition.y < -m_maxHeightDistance)
            {
                m_destroyEvent[index] = true;
                return;
            }
        }
        else {
            if (m_currentExistance[index].m_currentPosition.y > m_maxHeightDistance
                || m_currentExistance[index].m_currentPosition.y < 0f)
            {
                m_destroyEvent[index] = true;
                return;
            }
        }
        if (m_currentExistance[index].m_currentPosition.x > m_maxWidthDistance 
            || m_currentExistance[index].m_currentPosition.x < -m_maxWidthDistance)
        {
            m_destroyEvent[index] = true;
            return;
        }
        if (m_currentExistance[index].m_currentPosition.z > m_maxWidthDistance 
            || m_currentExistance[index].m_currentPosition.z < -m_maxWidthDistance)
        {
            m_destroyEvent[index] = true;
            return;
        }


    }
}





