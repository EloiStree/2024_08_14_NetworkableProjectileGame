using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericProjectileSpawnerMono : MonoBehaviour
{

    [Header("What type of projectile")]
    public ProjectilePoolManagerMono m_projectilePoolManager;

    [Header("Basic Spawner")]
    public Transform m_turretTip;
    public bool m_spawnerAllowToFire = true;
    public float m_timeBetweenProjectils = 0.5f;
    public float m_projectileSpeed = 10f;
    public float m_projectileRadius = 0.1f;

    [Header("Debug")]
    public string m_whenLastSpawnRequested;
    public void SetAsAllowedToFire(bool isAllowToFire)
    {
        m_spawnerAllowToFire = isAllowToFire;
    }
    public void OnEnable()
    {
        StartCoroutine(Coroutine());
    }
    public IEnumerator Coroutine() { 
    
        while (true)
        {
            if (m_spawnerAllowToFire) { 
                SpawnProjectile();
                yield return new WaitForSeconds(m_timeBetweenProjectils);
            }
            else {
                yield return new WaitForEndOfFrame(); 
            }
        }
    
    }
    [ContextMenu("Fire Projectile")]
    public void SpawnProjectile()
    {

        m_projectilePoolManager.Spawn(
            m_turretTip.position,
            m_turretTip.forward ,
            m_turretTip.rotation,
            m_projectileSpeed, 
            m_projectileRadius);
        m_whenLastSpawnRequested = DateTime.UtcNow.ToString("HH:mm:ss.fff");
    }
}
