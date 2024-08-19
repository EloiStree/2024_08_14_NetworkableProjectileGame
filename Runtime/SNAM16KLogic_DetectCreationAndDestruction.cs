
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

public class SNAM16KLogic_DetectCreationAndDestruction : MonoBehaviour {


    public SNAM16K_ObjectBool m_isExistingCurrentState;
    public SNAM16K_ObjectBool m_isExistingPreviousState;
    public SNAM16K_ObjectBool m_hadBeenCreatedDuringDelta;
    public SNAM16K_ObjectBool m_hadBeenDestroyDuringDelta;

    public UnityEvent<int> m_onIndexCreated;
    public UnityEvent<int> m_onIndexDestroyed;

    public int m_lastPushedCreated;
    public int m_lastPushedDestroyed;
    public bool m_useUpdate=true;
 
    private bool m_isInitiatized = false;
    public void BroadcastIndexOfCreatedAndDestroyProjectile()
    {
        if (!m_isInitiatized)
        {
            m_isInitiatized = true;
            InitCreate();
        }

        m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64).Complete();
        BroadcastCreatedAndDestroy();
    }

    public void BroadcastCreatedAndDestroy()
    {
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (m_hadBeenCreatedDuringDelta[i])
            {
                m_lastPushedCreated = i;
                m_onIndexCreated.Invoke(i);
            }
            if (m_hadBeenDestroyDuringDelta[i])
            {
                m_lastPushedDestroyed = i;
                m_onIndexDestroyed.Invoke(i);
            }
        }
    }

    public STRUCTJOB_FindCreatedAndDeleted m_job;
 

    private void InitCreate()
    {
        m_job = new STRUCTJOB_FindCreatedAndDeleted();
        m_job.m_isExistingCurrentState = m_isExistingCurrentState.GetNativeArrayHolder().GetNativeArray();
        m_job.m_isExistingPreviousState = m_isExistingPreviousState.GetNativeArrayHolder().GetNativeArray();
        m_job.m_hadBeenCreatedDuringDelta = m_hadBeenCreatedDuringDelta.GetNativeArrayHolder().GetNativeArray();
        m_job.m_hadBeenDestroyDuringDelta = m_hadBeenDestroyDuringDelta.GetNativeArrayHolder().GetNativeArray();
    }
}

[BurstCompile]
public struct STRUCTJOB_FindCreatedAndDeleted: IJobParallelFor
{

    public NativeArray<bool> m_isExistingCurrentState;
    public NativeArray<bool> m_isExistingPreviousState;

    [WriteOnly]
    public NativeArray<bool> m_hadBeenCreatedDuringDelta;
    [WriteOnly]
    public NativeArray<bool> m_hadBeenDestroyDuringDelta;

    public void Execute(int index)
    {
        if (m_isExistingCurrentState[index] && !m_isExistingPreviousState[index])
        {
            m_hadBeenCreatedDuringDelta[index] = true;
            m_hadBeenDestroyDuringDelta[index] = false;
        }
        else if (!m_isExistingCurrentState[index] && m_isExistingPreviousState[index])
        {
            m_hadBeenCreatedDuringDelta[index] = false;
            m_hadBeenDestroyDuringDelta[index] = true;
        }
        else {
            m_hadBeenCreatedDuringDelta[index] = false;
            m_hadBeenDestroyDuringDelta[index] = false;
        }
        m_isExistingPreviousState[index] = m_isExistingCurrentState[index];
    }
}
