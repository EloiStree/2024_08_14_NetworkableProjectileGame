using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRandomlyTargetMono : MonoBehaviour
{
    public Transform m_anchor;
    public Transform m_frontTopRightCorner;
    public Transform m_whatToMove;

    [ContextMenu("Move Randomly")]
    public void MoveRandomly()
    {
        if (m_anchor == null || m_frontTopRightCorner==null || m_whatToMove==null)
            return;

        Vector3 localPosition = new Vector3(
            Random.Range(-m_frontTopRightCorner.position.x, m_frontTopRightCorner.position.x),
            Random.Range(-m_frontTopRightCorner.position.y, m_frontTopRightCorner.position.y),
            Random.Range(-m_frontTopRightCorner.position.z, m_frontTopRightCorner.position.z)
        );

        Vector3 rotated= m_anchor.rotation * localPosition;
        Vector3 position = m_anchor.position + rotated;

        m_whatToMove.position = position;
    }
}
