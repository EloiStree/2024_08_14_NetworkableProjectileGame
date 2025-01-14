﻿using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct STRUCTJOB_ProjectileCollisionJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<bool> m_isUsed;
    [ReadOnly]
    public NativeArray<STRUCT_ProjectileCapsulePosition> m_capsuleMove;
    [WriteOnly]
    public NativeArray<bool> m_touchingTarget;

    public STRUCT_ProjectileCapsulePosition m_targetCapsule;
    public float m_targetInfluenceRadius;
    public void Execute(int index)
    {
        if (!m_isUsed[index])
            m_touchingTarget[index] = false;
          STRUCT_ProjectileCapsulePosition projectile = m_capsuleMove[index];


        //TRY TO DISQUALIFY PROJECTILES WITHOUT COMPLEX MATHEMATICS.
        //float td1 = Vector3.Distance(m_targetCapsule.m_currentPosition, projectile.m_currentPosition);
        //float td2 = Vector3.Distance(m_targetCapsule.m_previousPosition, projectile.m_previousPosition);

        //if (td1 > m_targetInfluenceRadius && td2 > m_targetInfluenceRadius)
        //{
        //    m_touchingTarget[index] = false;
        //    return;
        //}
       




        Vector3 shortestStartLineA, shortestEndLineB;
            CapsuleLineCollisionUtility.
            GetShortestLineBetweenTwoSections(
                out shortestStartLineA,
                out shortestEndLineB,
                m_targetCapsule.m_previousPosition,
                m_targetCapsule.m_currentPosition,
                projectile.m_previousPosition,
                projectile.m_currentPosition,
                false);
            Vector3 forward = (shortestEndLineB - shortestStartLineA);
            m_touchingTarget[index] = forward.magnitude < (projectile.m_capsuleRadius + m_targetCapsule.m_capsuleRadius);
           
    }
}