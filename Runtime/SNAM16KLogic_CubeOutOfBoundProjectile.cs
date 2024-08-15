using Eloi.WatchAndDate;
using Unity.Jobs;
using UnityEngine;

public class SNAM16KLogic_CubeOutOfBoundProjectile : MonoBehaviour
{
    public float m_squareWidth = ushort.MaxValue/1000f;
    public bool m_useDownPart;

    public Transform m_centerOfSpace;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ObjectBool m_hadBeenDestroy;
    public SNAM16K_ProjectileDestroyedEvent m_destroyEventHolder;
    public SNAM16K_ProjectileCapsulePosition m_projectileCapsule;

    public WatchAndDateTimeActionResult m_outOfBoxed;


    public bool m_useUpdate=true;
    public void Update()
    {
        if(m_useUpdate)
        Apply();
    }
    
    public void Apply()
    {

        m_outOfBoxed.StartCounting();
        STRUCTJOB_AsteroideOutOfBoundJob outOfBoundJob = new STRUCTJOB_AsteroideOutOfBoundJob();
        outOfBoundJob.m_destroyEvent = m_isUsedProjectil.GetNativeArray();
        outOfBoundJob.m_currentExistance = m_projectileCapsule.GetNativeArray();
        outOfBoundJob.m_currentMaxAsteroide = SNAM16K.ARRAY_MAX_SIZE;
        outOfBoundJob.m_centerPosition = m_centerOfSpace.position;
        outOfBoundJob.m_maxHeightDistance = m_squareWidth/2;
        outOfBoundJob.m_maxWidthDistance = m_squareWidth;
        outOfBoundJob.m_useDownPart = m_useDownPart;
        JobHandle outOfBoundJobHandle = outOfBoundJob.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64);
        outOfBoundJobHandle.Complete();
        m_outOfBoxed.StopCounting();
    }
}
