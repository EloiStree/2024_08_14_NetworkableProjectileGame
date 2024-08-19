using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleepy_ExplosionSpawner : MonoBehaviour
{

    public ProjectilePoolManagerMono m_pool;
    public Transform m_whereToExplode;
    public int m_bulletsPerExplosion = 5000;

    public float m_speed = 10;
    public float m_radius = 0.2f;
    private void Awake()
    {
        Explode();
    }



    [ContextMenu("Explode")]
    public void Explode()
    {
        for (int i = 0; i < m_bulletsPerExplosion; i++)
        {

            Vector3 dir= GetRandomForward();
            m_pool.Spawn(
                m_whereToExplode.position,
                dir,
                Quaternion.LookRotation(dir),
                m_speed, m_radius
                );
        }
    }

    private Vector3 GetRandomForward()
    {
        return new Vector3(
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f),
            UnityEngine.Random.Range(-1f, 1f)
            ).normalized;
    }
}
