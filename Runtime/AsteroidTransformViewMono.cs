using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class AsteroidTransformViewMono : MonoBehaviour {

    public AsteroideJobManagerMono m_manager;

    public Transform[] m_transformToUsed;

    public Transform m_hideTransform;

    private void Awake()
    {
        m_manager.AddCreationNewAsteroidIndex(ReactToAsteroidNewPosition);
        //for (int i = 0; i < m_transformToUsed.Length; i++)
        //{
        //    m_transformToUsed[i].gameObject.SetActive(false);
        //    m_transformToUsed[i].position = m_hideTransform.position;
        //    m_transformToUsed[i].rotation = m_hideTransform.rotation;
        //    m_transformToUsed[i].localScale = m_hideTransform.localScale;
        //}
        for (int i = 0; i < m_transformToUsed.Length; i++)
        {
            ReactToAsteroidNewPosition(i);
        }
    }

    private void ReactToAsteroidNewPosition(int index)
    {
        if(index< m_transformToUsed.Length)
        {
            AsteroidCreationEvent c = m_manager.m_asteroidInGame[index];
            Transform t = m_transformToUsed[index];
            t.gameObject.SetActive(true);
            t.position = c.m_startPosition;
            t.rotation = c.m_startRotationEuler;
            t.localScale= Vector3.one * c.m_colliderRadius * 2;
    
        }
        else
        {
            
        }
    }

    public void OnDestroy()
    {
        m_manager.RemoveCreationnewAsteroidIndex(ReactToAsteroidNewPosition);
    }



    public void Update()
    {
        AsteroideMoveApplyToTransform moveApplyToTransform = new AsteroideMoveApplyToTransform();
        moveApplyToTransform.m_currentExistance = m_manager.m_asteroidPosition;
        moveApplyToTransform.m_currentMaxAsteroide = m_manager.m_numberOfAsteroidsInGame;
        TransformAccessArray transformAccessArray = new TransformAccessArray(m_transformToUsed.Length);
        transformAccessArray.SetTransforms(m_transformToUsed);

        JobHandle moveApplyToTransformHandle = moveApplyToTransform.Schedule(transformAccessArray);
        moveApplyToTransformHandle.Complete();
        transformAccessArray.Dispose();
    }
}





