using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TeleportingCheckPointQuickScriptMono : MonoBehaviour
{

    public float m_minSize = 0.1f;
    public float m_maxSize = 1.0f;
    public Transform m_centerOfSpawningCube;
    public Transform m_cubeRootToMove;
    public bool m_useDefaultCollideWithPlayer = true;

    public int m_checkPointCount = 0;

    public UnityEvent<int> m_onCheckPointCountChanged;
    public UnityEvent<string> m_onCheckPointCountStringChanged;

    public LayerMask m_allowCollision;



    [ContextMenu("Teleport Randomly")]
    public void TeleportRandomly()
    {
        Transform t = m_centerOfSpawningCube;
        Vector3 whereToRespawn = t.position;
        whereToRespawn += t.forward* Random.Range(-t.localScale.z, t.localScale.z);
        whereToRespawn += t.up * Random.Range(-t.localScale.y, t.localScale.y);
        whereToRespawn += t.right * Random.Range(-t.localScale.x, t.localScale.x);
        m_cubeRootToMove.position= whereToRespawn;
        m_cubeRootToMove.rotation= Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        m_cubeRootToMove.localScale = Vector3.one * Random.Range(m_minSize, m_maxSize);
        m_checkPointCount++;
        m_onCheckPointCountChanged.Invoke(m_checkPointCount);
        m_onCheckPointCountStringChanged.Invoke(m_checkPointCount.ToString());
    }


    public void ResetScoreToZero()
    {
        m_checkPointCount = 0;
        m_onCheckPointCountChanged.Invoke(m_checkPointCount);
        m_onCheckPointCountStringChanged.Invoke(m_checkPointCount.ToString());
    }
    public void GetTeleportScore(out int score)
    {
        score = m_checkPointCount;
    }

    public GameObject m_lastCollision;

    
    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        CheckCollision(target);
    }

    private void CheckCollision(GameObject collision)
    {
        if (m_useDefaultCollideWithPlayer)
        {
            if (collision.GetComponentInChildren<CheckPointableTagMono>()
                || collision.GetComponent<CheckPointableTagMono>()
                || collision.GetComponentInParent<CheckPointableTagMono>())
            {
                TeleportRandomly();
                m_lastCollision = collision; 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        GameObject target = other.gameObject;
        CheckCollision(target);
    }
}
