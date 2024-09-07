using System;
using UnityEngine;

public class KeepPlayerInZoneMono : MonoBehaviour
{
    public Transform m_anchor;
    public Transform m_frontTopRightCorner;
    public Transform m_whatToMove;
    public Transform m_whereToRespawn;

    public float m_extraPadding = 0.1f;
    public bool m_useUpdate = true;
    public void Update()
    {
        if(m_useUpdate)
            CheckIfPlayerOutOfZone();
    }

    [ContextMenu("Is out of zone ?")]
    private void CheckIfPlayerOutOfZone()
    {
     
        if (m_anchor == null || m_frontTopRightCorner == null || m_whatToMove == null || m_whereToRespawn ==null)
            return;

        Vector3 playerLocaly = m_whatToMove.position - m_anchor.position;
        Vector3 local = Quaternion.Inverse(m_anchor.rotation) * playerLocaly;
        if(local.x > m_frontTopRightCorner.position.x+ m_extraPadding || local.x < -(m_frontTopRightCorner.position.x+m_extraPadding) ||
            local.y > m_frontTopRightCorner.position.y + m_extraPadding || local.y < -(m_frontTopRightCorner.position.y+ m_extraPadding) ||
            local.z > m_frontTopRightCorner.position.z + m_extraPadding || local.z < -(m_frontTopRightCorner.position.z+ m_extraPadding) )
        {
            m_whatToMove.position = m_whereToRespawn.position;
        }
    }
}