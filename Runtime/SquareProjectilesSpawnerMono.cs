using System;
using System.Collections;
using UnityEngine;

public class SquareProjectilesSpawnerMono : MonoBehaviour
{

    [Header("What type of projectile")]
    public ProjectilePoolManagerMono m_projectilePoolManager;

    [Header("Basic Spawner")]
    public Transform m_regionCenter;
    public Transform m_regionTopFrontRight;
    public bool m_spawnerAllowToFire = true;
    public float m_timeBetweenProjectilsMin = 0.05f;
    public float m_timeBetweenProjectilsMax = 0.6f;
    public float m_projectileSpeedMin = 5f;
    public float m_projectileSpeedMax = 20f;
    public float m_projectileRadiusMin = 0.1f;
    public float m_projectileRadiusMax = 0.5f;

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
    public IEnumerator Coroutine()
    {

        while (true)
        {
            if (m_spawnerAllowToFire)
            {
                SpawnProjectile();
                float time = UnityEngine.Random.Range(m_timeBetweenProjectilsMin, m_timeBetweenProjectilsMax);
                yield return new WaitForSeconds(time);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }

    }
    [ContextMenu("Fire Projectile")]
    public void SpawnProjectile()
    {

        float radius = UnityEngine.Random.Range(m_projectileRadiusMin, m_projectileRadiusMax);
        float speed=  UnityEngine.Random.Range(m_projectileSpeedMin, m_projectileSpeedMax);

        GetRandomPoint(out Vector3 positionStart);
        GetRandomPoint(out Vector3 positionEnd);

        Debug.DrawLine(positionStart, positionEnd, Color.cyan, 1f);

        Vector3 direction = positionEnd - positionStart;

        Quaternion rotRandom = Quaternion.Euler(
            UnityEngine.Random.Range(0, 360),
            UnityEngine.Random.Range(0, 360),
            UnityEngine.Random.Range(0, 360));

        m_projectilePoolManager.Spawn(
            positionStart,
            direction,
            rotRandom,
            speed,
            radius);
        m_whenLastSpawnRequested = DateTime.UtcNow.ToString("HH:mm:ss.fff");
    }

    private void GetRandomPoint(out Vector3 position)
    {


        
        Vector3 direction = m_regionTopFrontRight.position - m_regionCenter.position;
        Vector3 relocatedDirection = Quaternion.Inverse(m_regionCenter.rotation) * direction;

        Vector3 random = new Vector3(
            UnityEngine.Random.Range(-relocatedDirection.x, relocatedDirection.x), 
            UnityEngine.Random.Range(-relocatedDirection.y, relocatedDirection.y),
            UnityEngine.Random.Range(-relocatedDirection.z, relocatedDirection.z)
            );

        int flatRandom = UnityEngine.Random.Range(0, 6);

        if (flatRandom == 0)
            random = new Vector3(relocatedDirection.x, random.y, random.z);
        else if (flatRandom == 1)
            random = new Vector3(random.x, relocatedDirection.y, random.z);
        else if (flatRandom == 2)
            random = new Vector3(random.x, random.y, relocatedDirection.z);
        else if (flatRandom == 3)
            random = new Vector3(-relocatedDirection.x, random.y, random.z);
        else if (flatRandom == 4)
            random = new Vector3(random.x, -relocatedDirection.y, random.z);
        else if (flatRandom == 5)
            random = new Vector3(random.x, random.y, -relocatedDirection.z);

        Vector3 relocated = m_regionCenter.rotation * random;

        position = m_regionCenter.position + relocated;

    }
}
