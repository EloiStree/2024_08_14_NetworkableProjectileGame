
using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Events;

public class SNAM16KLogic_ProjectilesSoloCollision : MonoBehaviour{ 


    public SNAM16K_ObjectBool m_isUsedProjectile;
    public SNAM16K_ObjectBool m_isInCollision;
    public SNAM16K_ProjectileCapsulePosition m_projectiles;
    public Transform m_targetCenterPoint;
    public Transform m_targetRadiusPoint;
    [Header("Debug")]
    public float m_targetRadius;
    public float m_influenceRadius;
    public STRUCT_ProjectileCapsulePosition m_targetPosition;
    public bool m_isTouched;
    public UnityEvent m_onTouchEnter;
    public UnityEvent m_onTouchExit;
    public UnityEvent<bool> m_onTouching;
    public string m_lastTouchedEvent;
    public void Compute() { 
    
        m_targetRadius = Vector3.Distance(m_targetCenterPoint.position, m_targetRadiusPoint.position);

        m_targetPosition.m_previousPosition = m_targetPosition.m_currentPosition;
        m_targetPosition.m_currentPosition = m_targetCenterPoint.position;
        m_targetPosition.m_capsuleRadius = m_targetRadius;
        m_influenceRadius = m_targetRadius*2f + 
            Vector3.Distance(
                m_targetPosition.m_previousPosition,
                m_targetPosition.m_currentPosition);

        STRUCTJOB_ProjectileCollisionJob job = new STRUCTJOB_ProjectileCollisionJob();
        job.m_isUsed = m_isUsedProjectile.GetNativeArrayHolder().GetNativeArray();
        job.m_capsuleMove = m_projectiles.GetNativeArrayHolder().GetNativeArray();
        job.m_touchingTarget = m_isInCollision.GetNativeArrayHolder().GetNativeArray();
        job.m_targetCapsule = m_targetPosition;
        job.m_targetInfluenceRadius = m_influenceRadius;
        JobHandle handle = job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64);
        handle.Complete();

        bool isTouched = false;
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (job.m_touchingTarget[i])
            {
                Debug.DrawLine(job.m_capsuleMove[i].m_currentPosition, job.m_capsuleMove[i].m_previousPosition,  Color.magenta,10);
                isTouched = true;
                break;
            }
        }
        Debug.DrawLine(m_targetPosition.m_previousPosition, m_targetPosition.m_currentPosition, isTouched? Color.green: Color.red);

        
        if (m_isTouched != isTouched) { 
            m_isTouched = isTouched;
            m_lastTouchedEvent = DateTime.UtcNow.Ticks.ToString();
            if (m_isTouched)
                m_onTouchEnter.Invoke();
            m_onTouching.Invoke(m_isTouched);
            if(!isTouched)
                m_onTouchExit.Invoke();
        }
    
    }
}
