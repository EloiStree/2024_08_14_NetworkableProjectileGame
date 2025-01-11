using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eloi.SNAM;

public class SNAM16KLogic_DebugCollisionFromIndex : MonoBehaviour
{

    public Transform m_targetCenter;
    public Transform m_targetRadiusPoint;

    public SNAM16K_ObjectVector3 m_position;
    public SNAM16K_ProjectileCreatedEvent m_projectileCreated;


    public float m_playerToCollisionRadius;
    public float m_playerRadius;
    public float m_sphereRadius;
    public STRUCT_ProjectileCreationEvent m_lastCollision;

    public float m_timeDisplay;
    public void PushInIndex(int index)
    {
        if (index < SNAM16K.ARRAY_MAX_SIZE) {
            float radius = Vector3.Distance(m_targetCenter.position, m_targetRadiusPoint.position);
            Vector3 projectile, player;
            projectile = m_position[index];
            player = m_targetCenter.transform.position;
            Vector3 dir = (player - projectile).normalized * m_projectileCreated[index].m_colliderRadius;
            Vector3 inverseDir= (projectile - player).normalized* radius;

            Debug.DrawLine(projectile, player, Color.red, m_timeDisplay);
            Debug.DrawLine(projectile, projectile+dir, Color.yellow, m_timeDisplay);
            Debug.DrawLine(player, player+inverseDir , Color.yellow*0.5f, m_timeDisplay);

            m_playerToCollisionRadius = Vector3.Distance(projectile, player);
            m_playerRadius = radius;
            m_sphereRadius = m_projectileCreated[index].m_colliderRadius;
            m_lastCollision = m_projectileCreated[index];


        }
    }
    
}
