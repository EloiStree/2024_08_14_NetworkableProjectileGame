using PlasticGui.WorkspaceWindow.Locks;
using System;
using Unity.Jobs;
using UnityEngine;

public class SNAM16KLogic_MoveProjectiles : MonoBehaviour
{
    public SNAM16K_ObjectBool m_isUsedProjectile;
    public SNAM16K_ObjectVector3 m_projectilePosition;
    public SNAM16K_ProjectileCapsulePosition m_projectileCapsule;
    public SNAM16K_ProjectileMoveConstant m_projectileMoveInformation;

    public Transform m_hiddenWhenNotUsed;

    public long m_now;
    public long m_previous;

    public bool m_useUpdate;
    public void Update()
    {
        if (m_useUpdate)
            Apply();
    }
    public void Apply()
    {

        m_previous= m_now;
        m_now = DateTime.UtcNow.Ticks;
        STRUCTJOB_ProjectileMoveJob job = new STRUCTJOB_ProjectileMoveJob();
        job.m_isUsed = m_isUsedProjectile.GetNativeArray();
        job.m_projectileInGame=m_projectileMoveInformation.GetNativeArray();
        job.m_currentExistance= m_projectileCapsule.GetNativeArray();
        job.m_currentMaxAsteroide=SNAM16K.ARRAY_MAX_SIZE;
        job.m_serverCurrentUtcNowTicks=m_now;
        job.m_serverCurrentUtcPreviousTicks=m_previous;
        job.m_currentPosition= m_projectilePosition.GetNativeArray();
        job.m_hidePosition = m_hiddenWhenNotUsed ? m_hiddenWhenNotUsed.position : Vector3.one * float.MaxValue;
        JobHandle handle = job.Schedule(m_projectileMoveInformation.GetLength(), 64);
        handle.Complete();

    }   
}
