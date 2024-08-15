using UnityEngine;
using UnityEngine.Events;

public class SNAM16KLogic_DetectCreationAndDestruction : MonoBehaviour {


    public SNAM16K_ObjectBool m_isExisting;
    public SNAM16K_ObjectBool m_isExistingPrevious;
    public SNAM16K_ObjectBool m_isCreated;
    public SNAM16K_ObjectBool m_isDestroyed;

    public UnityEvent<int> m_onIndexCreated;
    public UnityEvent<int> m_onIndexDestroyed;

    public bool m_applyCurrentToPrevious = true;
    public bool m_useUpdate=true;
    public void Update()
    {
        if(m_useUpdate)
        Apply();
    }
    public void Apply() { 
    
        for(int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (m_isExisting[i] && !m_isExistingPrevious[i])
            {
                m_isCreated[i] = true;
                m_isDestroyed[i] = false;
            }
            else if (!m_isExisting[i] && m_isExistingPrevious[i])
            {
                m_isCreated[i] = false;
                m_isDestroyed[i] = true;
            }
            if(m_applyCurrentToPrevious)
                m_isExistingPrevious[i] = m_isExisting[i];
        }
        for (int i = 0; i < SNAM16K.ARRAY_MAX_SIZE; i++)
        {
            if (m_isCreated[i])
            {
                m_onIndexCreated.Invoke(i);
            }
            if (m_isDestroyed[i])
            {
                m_onIndexDestroyed.Invoke(i);
            }
        }
    }



}
