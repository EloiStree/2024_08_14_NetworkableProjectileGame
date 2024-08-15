using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEditor.Progress;

public class SphereAroundPlayerMono : MonoBehaviour
{

    public Transform m_playerReference;
    public SNAM16K_ObjectVector3 m_spherePosition;
    public SNAM16k_ItemIndexDistance m_itemDistance;
    public SNAM16K_AsteroidCreatedEvent m_asteroidInfo;

    public float m_scopeDistance = 10;
    public List<STRUCT_ItemObjectDistance>  m_nearItem;
    public int m_usePoolLimiter = 3;
    public GameObject[] m_pools;
    public void Update()
    {

        STRUCTJOB_ComputeObjectsDistance job = new STRUCTJOB_ComputeObjectsDistance();
        job.m_targetPosition = m_playerReference.position;
        job.m_spherePosition = m_spherePosition.GetNativeArray();
        job.m_distanceComputed = m_itemDistance.GetNativeArray();
        job.Schedule(SNAM16K.ARRAY_MAX_SIZE, 64).Complete();

        m_nearItem= job.m_distanceComputed.Where<STRUCT_ItemObjectDistance>(k => k.m_distance < m_scopeDistance).ToList();
        m_nearItem.Sort( (x, y) => (x.m_distance.CompareTo(y.m_distance)));


        int nearItemIndex = 0;
        for (int i = 0; i< m_pools.Length ; i++)
        {
            bool objectActive = false;
            if ( i < m_nearItem.Count )
            {
                if (nearItemIndex < m_pools.Length) { 
                    m_pools[i].transform.position = m_spherePosition[nearItemIndex];
                    m_pools[i].transform.localScale = Vector3.one * m_asteroidInfo[nearItemIndex].m_colliderRadius*2f;
                    objectActive = true;
                    Debug.DrawLine(m_playerReference.position, m_pools[i].transform.position,Color.green, Time.deltaTime);
                    nearItemIndex++;

                }
            }

            m_pools[i].gameObject.SetActive(objectActive);
        }
    }
  
}

[System.Serializable]
public struct STRUCT_ItemObjectDistance 
{
 
    public int m_index;
    public float m_distance;
  

}

public struct STRUCTJOB_ComputeObjectsDistance : IJobParallelFor
{
    public Vector3 m_targetPosition;
    public NativeArray<Vector3> m_spherePosition;
    public NativeArray<STRUCT_ItemObjectDistance> m_distanceComputed;
    public void Execute(int index)
    {
        STRUCT_ItemObjectDistance d = new STRUCT_ItemObjectDistance();
        d.m_index = index;
        d.m_distance = Vector3.Distance(m_spherePosition[index], m_targetPosition);
        m_distanceComputed[index] = d;
    }
}