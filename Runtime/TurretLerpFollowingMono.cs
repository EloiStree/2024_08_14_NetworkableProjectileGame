using UnityEngine;

public class TurretLerpFollowingMono : MonoBehaviour
{
    public Transform m_whatToRotate;
    public Transform m_target;
    public float m_lerpSpeed = 1f;

    public void Update()
    {
        if( m_target == null)
            return;
        if( m_whatToRotate == null )
            return;
        Quaternion targetRotation = Quaternion.LookRotation(m_target.position - m_whatToRotate.position);
        m_whatToRotate.rotation = Quaternion.Lerp(m_whatToRotate.rotation, targetRotation, Time.deltaTime * m_lerpSpeed);
    }
}