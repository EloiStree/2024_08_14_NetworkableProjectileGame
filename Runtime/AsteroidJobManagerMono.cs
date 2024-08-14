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

public class AsteroideJobManagerMono : MonoBehaviour
{

    public byte m_poolId=1;
    public int m_poolItemMax = 128*128;
    public int m_numberOfAsteroidsInGame = 20;
    public NativeArray<bool> m_asteroidDestroyedEvent;
    public NativeArray<bool> m_asteroidCreationEvent;
    public NativeArray<AsteroidCreationEvent> m_asteroidInGame;
    public NativeArray<AsteroidMoveConstant> m_asteroidMoveUpdateInfo;
    public NativeArray<AsteroidCapsulePosition> m_asteroidPosition;


    public Transform m_centerOfSpace;

    public float m_skyHeight=ushort.MaxValue/2f;
    public float m_squareWidth = ushort.MaxValue / 2f;

    public UnityEvent<AsteroidCreationEvent> m_onAsteroidCreated;
    public UnityEvent<AsteroidDestructionEvent> m_onAsteroidDestroyed;

    public float m_minSpeed=1;
    public float m_maxSpeed=10;
    public float m_minSize = 0.1f;
    public float m_maxSize = 1f;

    public void OnEnable()
    {
        m_asteroidDestroyedEvent = new NativeArray<bool>(m_poolItemMax, Allocator.Persistent);
        m_asteroidCreationEvent = new NativeArray<bool>(m_poolItemMax, Allocator.Persistent);
        m_asteroidMoveUpdateInfo= new NativeArray<AsteroidMoveConstant>(m_poolItemMax, Allocator.Persistent);
        m_asteroidInGame = new NativeArray<AsteroidCreationEvent>(m_poolItemMax, Allocator.Persistent);
        m_asteroidPosition = new NativeArray<AsteroidCapsulePosition>(m_poolItemMax, Allocator.Persistent);

        RandomizedAll();
    }

    [ContextMenu("Randomized All")]
    public void RandomizedAll()
    {
        for (int i = 0; i < m_poolItemMax; i++)
        {
            AsteroidCreationEvent a = m_asteroidInGame[i];
            AsteroidMoveConstant m= m_asteroidMoveUpdateInfo[i];
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

    private void SetRandomStartPointTo(ref AsteroidCreationEvent asteroidCreationEvent, ref AsteroidMoveConstant moveInfo)
    {
        asteroidCreationEvent.m_startPosition = new Vector3(UnityEngine.Random.Range(-m_squareWidth, m_squareWidth), UnityEngine.Random.Range(0, m_skyHeight), UnityEngine.Random.Range(-m_squareWidth, m_squareWidth));
        asteroidCreationEvent.m_startRotationEuler = Quaternion.Euler(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        asteroidCreationEvent.m_startDirection = asteroidCreationEvent.m_startRotationEuler * Vector3.forward;
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
    public void Update()
    {
        m_moveObject.StartCounting();
        m_updateTick++;
        m_currentTickServerUtcPrevious = m_currentTickServerUtcNow;
        m_currentTickServerUtcNow = DateTime.UtcNow.Ticks;
        AsteroideMoveJob moveJob = new AsteroideMoveJob();
        moveJob.m_asteroidInGame = m_asteroidMoveUpdateInfo;
        moveJob.m_currentExistance = m_asteroidPosition;
        moveJob.m_currentMaxAsteroide = m_numberOfAsteroidsInGame;
        moveJob.m_serverCurrentUtcNowTicks = m_currentTickServerUtcNow;
        moveJob.m_serverCurrentUtcPreviousTicks = m_currentTickServerUtcPrevious;
        JobHandle moveJobHandle = moveJob.Schedule(m_numberOfAsteroidsInGame, 64);
        moveJobHandle.Complete();

       m_moveObject.StopCounting();
        m_outOfBoxed.StartCounting();
        AsteroideOutOfBoundJob outOfBoundJob = new AsteroideOutOfBoundJob();
        outOfBoundJob.m_destroyEvent = m_asteroidDestroyedEvent;
        outOfBoundJob.m_currentExistance = m_asteroidPosition;
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
                AsteroidCreationEvent a = m_asteroidInGame[i];
                AsteroidMoveConstant m = m_asteroidMoveUpdateInfo[i];
                SetRandomStartPointTo(ref a, ref m);

                m_asteroidMoveUpdateInfo[i] = m;
                m_asteroidInGame[i] = a;
                m_onAsteroidDestroyed.Invoke(new AsteroidDestructionEvent() { 
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
    

    public void OnDisable()
    {
        m_asteroidDestroyedEvent.Dispose();
        m_asteroidCreationEvent.Dispose();
        m_asteroidInGame.Dispose();
        m_asteroidPosition.Dispose();
        m_asteroidMoveUpdateInfo.Dispose();
    }

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
public struct AsteroidCreationEvent {
    public byte m_poolId;
    public int m_poolItemIndex;
    public long m_serverUtcNowTicks;
    public Vector3 m_startPosition;
    public Quaternion m_startRotationEuler;
    public Vector3 m_startDirection;
    public float m_speedInMetersPerSecond;
    public float m_colliderRadius;
}

[System.Serializable]
public struct AsteroidMoveConstant {

    public long m_startUtcNowTicks;
    public float m_speedInMetersPerSecond;
    public Vector3 m_startPoint;
    public Vector3 m_direction;
}



[System.Serializable]
public struct AsteroidDestructionEvent { 
    public byte m_poolId;
    public int m_poolItemIndex;
    public long m_serverUtcNowTicks;
}

public struct AsteroidCapsulePosition
{
    public Vector3 m_currentPosition;
    public Vector3 m_previousPosition;
    public float m_capsuleRadius;
}

[BurstCompile]
public struct AsteroideMoveJob : IJobParallelFor
{

    [ReadOnly]
    public NativeArray<AsteroidMoveConstant> m_asteroidInGame;

    public NativeArray<AsteroidCapsulePosition> m_currentExistance;

    public int m_currentMaxAsteroide;
    public long m_serverCurrentUtcNowTicks;
    public long m_serverCurrentUtcPreviousTicks;
    public const float m_tickToSeconds= 1f/(float)TimeSpan.TicksPerSecond;

    public void Execute(int index)
    {
        if (index >= m_currentMaxAsteroide)
        {
            return;
        }
        AsteroidMoveConstant a = m_asteroidInGame[index];
        AsteroidCapsulePosition p = m_currentExistance[index];
     
        p.m_previousPosition = p.m_currentPosition;
        float timeSinceStart = 
            (m_serverCurrentUtcNowTicks - m_asteroidInGame[index].m_startUtcNowTicks)*m_tickToSeconds;
        float distance = m_asteroidInGame[index].m_speedInMetersPerSecond * timeSinceStart;
        p.m_currentPosition= a.m_startPoint+ a.m_direction * distance;
        m_currentExistance[index] = p;

      
    }
}


public struct AsteroideMoveApplyToTransform : IJobParallelForTransform
{
    public NativeArray<AsteroidCapsulePosition> m_currentExistance;
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


public struct AsteroideOutOfBoundJob : IJobParallelFor
{
    public NativeArray<bool> m_destroyEvent;
    public NativeArray<AsteroidCapsulePosition> m_currentExistance;
    public long m_serverCurrentUtcNowTicks;
    public long m_serverCurrentUtcPreviousTicks;

    public int m_currentMaxAsteroide;
    public Vector3 m_centerPosition;
    public float m_maxHeightDistance;
    public float m_maxWidthDistance;

    public void Execute(int index)
    {
        m_destroyEvent[index] = false;
        if (index>=m_currentMaxAsteroide)
        {
            return;
        }
        if(m_currentExistance[index].m_currentPosition.y > m_maxHeightDistance 
            || m_currentExistance[index].m_currentPosition.y <0f)
        {
            m_destroyEvent[index] = true;
            return;
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





