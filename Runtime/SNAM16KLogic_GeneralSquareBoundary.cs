using Unity.Jobs;
using UnityEngine;

using Eloi.SNAM;
public class SNAM16KLogic_GeneralSquareBoundary : MonoBehaviour
{
    public float m_maxDistance = ushort.MaxValue/2000;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ObjectVector3 m_projectilePosition;

    

    STRUCTJOB_DisableProjectileOutOfSquareLimite m_job;
    private void Start()
    {
        Init();



    }

    public void Init()
    {
        m_job = new STRUCTJOB_DisableProjectileOutOfSquareLimite();
        m_job.m_isUsedProjectil = m_isUsedProjectil.GetNativeArrayHolder().GetNativeArray();
        m_job.m_projectilePosition = m_projectilePosition.GetNativeArrayHolder().GetNativeArray();
        m_job.m_maxDistance = m_maxDistance;
    }

    public void DisableProjectileOutOfLimite()
    {
        m_job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64).Complete();


    }
}
