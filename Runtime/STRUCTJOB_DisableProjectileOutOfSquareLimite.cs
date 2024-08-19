using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct STRUCTJOB_DisableProjectileOutOfSquareLimite: IJobParallelFor
{
    public NativeArray<bool> m_isUsedProjectil;
    [ReadOnly]
    public NativeArray<Vector3> m_projectilePosition;
    public float m_maxDistance;

    public void Execute(int index)
    {
        if (m_isUsedProjectil[index])
        {
            Vector3 p = m_projectilePosition[index];
            if (p.x < -m_maxDistance || p.x > m_maxDistance ||
                p.y < -m_maxDistance || p.y > m_maxDistance ||
                p.z < -m_maxDistance || p.z > m_maxDistance)
            {
                m_isUsedProjectil[index] = false;
            }
        }
    }
}