using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DroneIMMO;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
using UnityEngine.Events;
public class SNAM16K_SphereColliderToSphereCollisions : MonoBehaviour
{

    public SphereCollider m_target;
    public SNAM16K_ObjectBool m_isActive;
    public SNAM16K_ObjectVector3 m_sphereCenter;
    public SNAM16K_ObjectFloat m_sphereRadius;
    public SNAM16K_ObjectBool m_hadColliding;

    public void Update()
    {
        STRUCTJOB_CheckCollision job = new STRUCTJOB_CheckCollision();
        job.m_targetPosition = m_target.transform.position;
        job.m_targetRadius = m_target.radius;
        job.m_positions = m_sphereCenter.GetNativeArray();
        job.m_radius = m_sphereRadius.GetNativeArray();
        job.m_isActive = m_isActive.GetNativeArray();
        job.m_isColliding = m_hadColliding.GetNativeArray();

        JobHandle jobHandle = job.Schedule(m_sphereCenter.GetNativeArray().Length, 64);
        jobHandle.Complete();


    }

    public struct STRUCTJOB_CheckCollision :IJobParallelFor {

        public Vector3 m_targetPosition;
        public float m_targetRadius;
        public NativeArray<Vector3> m_positions;
        public NativeArray<float> m_radius;
        public NativeArray<bool> m_isActive;
        public NativeArray<bool> m_isColliding;


        

        public void Execute(int index)
        {
            if (m_isActive[index] && m_radius[index]>0)
            {
                float radius = m_radius[index];
                Vector3 sphereCenter = m_positions[index];
                float distance = Vector3.Distance(m_targetPosition, sphereCenter);
                if (distance < radius + m_targetRadius)
                {
                    m_isColliding[index] = true;
                }
                else
                {
                    m_isColliding[index] = false;
                }
            }
        }
    }
}
