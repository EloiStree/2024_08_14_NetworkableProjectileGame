using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpawnerMono : MonoBehaviour
{


    public Transform m_turretTip;
    public float m_timeBetweenProjectils = 0.5f;
    public float m_projectileSpeed = 10f;
    public float m_projectileRadius = 0.1f;
    public ProjectilePoolManagerMono m_projectilePoolManager;


    public string m_lastFire;

    public IEnumerator Start() { 
    
        while (true)
        {
            yield return new WaitForSeconds(m_timeBetweenProjectils);
            SpawnProjectile();
        }
    
    }

    [ContextMenu("Fire Projectile")]
    public void SpawnProjectile()
    {

        m_projectilePoolManager.Spawn(m_turretTip.position, m_turretTip.forward , m_turretTip.rotation, m_projectileSpeed, m_projectileRadius);
        m_lastFire = DateTime.UtcNow.ToString("HH:mm:ss.fff");
    }
}
