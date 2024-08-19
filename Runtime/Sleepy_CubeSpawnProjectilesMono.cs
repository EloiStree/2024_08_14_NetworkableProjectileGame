using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleepy_CubeSpawnProjectilesMono : MonoBehaviour
{

    [Header("What type of projectile")]
    public ProjectilePoolManagerMono m_projectilePoolManager;

    [Header("Basic Spawner")]
    public Transform m_center;
    public float m_spawnRadius=10;
    public float m_spawnAngle=90;
    public float m_spawnRate=1;
    public float m_spawnSpeed=1;
    public float m_spawnSize=1;

    

    public void OnEnable()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        while (true) { 
        
            yield return new WaitForSeconds(m_spawnRate);
            yield return new WaitForEndOfFrame();
            Vector3 randomEuler=  new Vector3(R360(), R360(), R360());
            Vector3 forward= Quaternion.Euler(randomEuler) * Vector3.forward;
            Vector3 position= m_center.position + forward*m_spawnRadius ;

            Vector3 randomDire= Quaternion.Euler(RR(), RR(), RR()) * -Vector3.forward;

            m_projectilePoolManager.Spawn(position, randomDire, Quaternion.Euler(randomEuler), m_spawnSpeed, m_spawnSize);

        }
    }

    private float RR()
    {
        return UnityEngine.Random.Range(-m_spawnAngle/2f, m_spawnAngle / 2f);
    }

    private float R360()
    {
        return UnityEngine.Random.Range(0, 360);
    }
}
