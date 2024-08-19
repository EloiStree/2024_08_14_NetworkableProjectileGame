using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Split16K_CapsulePositionsToVector3 : MonoBehaviour
{
    
    public SNAM16K_ProjectileCapsulePosition m_capsulePositions;
    public SNAM16K_ObjectVector3 m_current;
    public SNAM16K_ObjectVector3 m_previous;
    public bool m_useUpdate=true;
    public void Update()
    {
        if (m_useUpdate) { 
            SplitCapsuleToCurrentPreviousPosition();
        }
    }
    STRUCTJOB_Split16K_CapsulePositionsToVector3 m_job;

    public void Awake()
    {
        m_job = new STRUCTJOB_Split16K_CapsulePositionsToVector3();
        m_job.m_capsulePositions = m_capsulePositions.GetNativeArrayHolder().GetNativeArray();
        m_job.m_currentPosition = m_current.GetNativeArrayHolder().GetNativeArray();
        m_job.m_previousPosition = m_previous.GetNativeArrayHolder().GetNativeArray();
    }


    public void SplitCapsuleToCurrentPreviousPosition()
    {
        m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64).Complete();
    }
    [BurstCompile]
    public struct STRUCTJOB_Split16K_CapsulePositionsToVector3 : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<STRUCT_ProjectileCapsulePosition> m_capsulePositions;

        [WriteOnly]
        public NativeArray<Vector3> m_currentPosition;

        [WriteOnly]
        public NativeArray<Vector3> m_previousPosition;
        public void Execute(int index)
        {
            m_currentPosition[index] = m_capsulePositions[index].m_currentPosition;
            m_previousPosition[index] = m_capsulePositions[index].m_previousPosition;
        }
    }
}
