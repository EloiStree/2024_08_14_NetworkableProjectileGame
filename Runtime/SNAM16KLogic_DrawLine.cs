using Eloi.WatchAndDate;
using UnityEngine;

public class SNAM16KLogic_DrawLine : MonoBehaviour
{

    public bool m_useDebugDraw=true;
    public bool m_useUpdate=true;
    public SNAM16K_ObjectBool m_isUsedProjectil;
    public SNAM16K_ProjectileCapsulePosition m_projectileCapsule;

    public Color m_color = Color.magenta;
    public WatchAndDateTimeActionResult m_applyTime;

    public void Update()
    {
        if (m_useUpdate)
            Apply();
    }
    public void Apply()
    {

        m_applyTime.StartCounting();
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (m_isUsedProjectil[i])
            {
                Vector3 pos = m_projectileCapsule[i].m_currentPosition;
                Vector3 pos2 =  m_projectileCapsule[i].m_previousPosition;
                Debug.DrawLine(pos, pos2, m_color);
            }
        }
        m_applyTime.StopCounting();
    }
}