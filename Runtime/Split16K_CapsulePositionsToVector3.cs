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
            Split();
        }
    }

    public void Split()
    {
        STRUCTJOB_Split16K_CapsulePositionsToVector3 job = new STRUCTJOB_Split16K_CapsulePositionsToVector3();
        job.m_capsulePositions = m_capsulePositions.GetNativeArray();
        job.m_currentPosition = m_current.GetNativeArray();
        job.m_previousPosition = m_previous.GetNativeArray();
        job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64).Complete();
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
