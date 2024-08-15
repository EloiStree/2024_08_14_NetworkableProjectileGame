using UnityEngine;

public class RespawnPlayerAtStartPointMono :MonoBehaviour {

    public Transform m_playerToMove;
    public Transform m_whereToRespawn;

    [ContextMenu("RespawnPlayer")]
    public void RespawnPlayer()
    {
        m_playerToMove.position = m_whereToRespawn.position;
    }
}