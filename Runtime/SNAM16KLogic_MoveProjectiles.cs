
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
    public STRUCTJOB_ProjectileMoveJob m_job;
    public void Awake()
    {
        m_now = DateTime.UtcNow.Ticks;
        m_previous = m_now;
        Invoke("Refresh", 1);
    }

    public void Refresh()
    {
        m_now = DateTime.UtcNow.Ticks;
        m_previous = m_now;
        m_job = new STRUCTJOB_ProjectileMoveJob();
        m_job.m_isUsed = m_isUsedProjectile.GetNativeArrayHolder().GetNativeArray();
        m_job.m_projectileInGame = m_projectileMoveInformation.GetNativeArrayHolder().GetNativeArray();
        m_job.m_currentExistance = m_projectileCapsule.GetNativeArrayHolder().GetNativeArray();
        m_isInit = true;
    }

    private bool m_isInit;
    public void Update()
    {
        if (m_useUpdate)
            Apply();
    }
    public void Apply()
    {
        if (!m_isInit)
            return;
        m_previous= m_now;
        m_now = DateTime.UtcNow.Ticks;
        m_job.m_currentMaxAsteroide=SNAM16K.ARRAY_MAX_SIZE;
        m_job.m_serverCurrentUtcNowTicks=m_now;
        m_job.m_serverCurrentUtcPreviousTicks=m_previous;
        m_job.m_hidePosition = m_hiddenWhenNotUsed ? m_hiddenWhenNotUsed.position : Vector3.one * float.MaxValue;
        JobHandle handle = m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64);
        handle.Complete();

    }   
}
