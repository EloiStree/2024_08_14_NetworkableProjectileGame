using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FetchFacadeStayAliveExerciceV0Mono : A_FacadeStayAliveExerciceMono
{


    [Header("Meta information on the current game")]
    public string m_droneName = "Tello Root of Knowledge";
    public string m_droneCodeId = "RootOfKnowledge_2024_08_15";
    public string m_droneGitCodeUrl = "https://github.com/EloiStree/2023_02_19_KidToyDroneTelloModeCode";

    public string m_stageEnvironementName = "Stay Alive Empty 32x32";
    public string m_stageEnvironementGuid = Guid.NewGuid().ToString();
    public string m_stageEnvironementGitHubUrl = "";

    [Header("Fetch anchor for information")]
    public Transform m_playerPosition;
    public SphereCollider m_playerRadius;
    public Transform m_checkPointToReach;
    public LayerMask m_stageLayerMask;
    public SNAM16K_ObjectVector3 m_projectilsPosition;

    [Header("Gamepad state")]
    [Range(-1f,1f)]
    public float m_joystickLeftHorizontal;
    [Range(-1f, 1f)]
    public float m_joystickLeftVertical;
    [Range(-1f, 1f)] 
    public float m_joystickRightHorizontal;
    [Range(-1f, 1f)] 
    public float m_joystickRightVertical;
    public UnityEvent<float> m_onJoystickLeftHorizontal;
    public UnityEvent<float> m_onJoystickLeftVertical;
    public UnityEvent<float> m_onJoystickRightHorizontal;
    public UnityEvent<float> m_onJoystickRightVertical;

    public List<I_ProjectileSpawnListener> m_projectileSpawnListeners = new List<I_ProjectileSpawnListener>();
    public List<Action> m_restartLevelListeners = new List<Action>();
    public List<Action> m_stageChangeListeners = new List<Action>();
    public UnityEvent m_onRestartLevel;
    public UnityEvent m_onStageChange;

    public float m_currentSurvivalTime = 0f;
    public int m_checkPointCount = 0;

    public bool m_userWantDefaultInput = true;
    public UnityEvent<bool> m_onDefaultInputEnable;


    public void SetSurviveTime(float surviveTime) { 
    
        m_currentSurvivalTime = surviveTime;
    }
    public void SetCheckPointCount(int checkPointCount)
    {
        m_checkPointCount = checkPointCount;
    }

    public void NotifySpawnProjectil(STRUCT_ProjectileCreationEvent creationEvent)
    {
        foreach (var listener in m_projectileSpawnListeners)
        {
            listener.OnProjectileSpawned(
                creationEvent.m_poolId,
                creationEvent.m_poolItemIndex,
                creationEvent.m_serverUtcNowTicks,
                creationEvent.m_startPosition,
                creationEvent.m_startRotation,
                creationEvent.m_startDirection,
                creationEvent.m_speedInMetersPerSecond,
                creationEvent.m_colliderRadius              
                );
        }
    }


    private void Awake()
    {

        NotifyLevelRestart();
        NotifyStageChanged();
        ActiveDefaultInputToFlyWithoutCode(m_userWantDefaultInput);
    }

    private void NotifyStageChanged()
    {
        foreach (var listener in m_stageChangeListeners)
        {
            if(listener!=null)
            listener.Invoke();
        }
        m_onStageChange.Invoke();
    }

    private void NotifyLevelRestart()
    {
        foreach (var listener in m_restartLevelListeners)
        {
            if(listener!=null)
            listener.Invoke();
        }
        m_onRestartLevel.Invoke();
    }

    #region ABSTRACT TO IMPLEMENT
    public override void AddProjectileSpawnListener(I_ProjectileSpawnListener listener)
    {
        m_projectileSpawnListeners.Add(listener);
    }

    public override void AddRestartLevelListener(Action listener)
    {
        m_restartLevelListeners.Add(listener);
    }

    public override void AddStageChangeListener(Action listener)
    {
        m_stageChangeListeners.Add(listener);
    }

    public override void GetAllProjectilesPositions(out NativeArray<Vector3> positions)
    {
        positions = m_projectilsPosition.GetNativeArrayHolder().GetNativeArray();
    }

    public override void GetCheckpointCount(out int checkpointCount)
    {
        checkpointCount = m_checkPointCount;
    }

    public override void GetDroneCodeBehaviourGuid(out string droneCodeId)
    {
        droneCodeId = m_droneCodeId;
    }

    public override void GetDroneGitHubCodeUrl(out string droneGitHubCodeUrl)
    {   
        droneGitHubCodeUrl = m_droneGitCodeUrl;
    }

    public override void GetDroneProductName(out string droneProductName)
    {
        droneProductName = m_droneName;
    }

    public override void GetJoystickLeftHorizontal(out float value1to1)
    {
        value1to1 = m_joystickLeftHorizontal;
    }

    public override void GetJoystickLeftVertical(out float value1to1)
    {
        value1to1 = m_joystickLeftVertical;
    }

    public override void GetJoystickRightHorizontal(out float value1to1)
    {
        value1to1 = m_joystickRightHorizontal;
    }

    public override void GetJoystickRightVertical(out float value1to1)
    {
        value1to1 = m_joystickRightVertical;
    }

    public override void GetNextCheckpoint(out Vector3 checkpointPosition, out Quaternion checkpointPositionDirection, out float circleRadius)
    {
        checkpointPosition = m_checkPointToReach.position;
        checkpointPositionDirection = m_checkPointToReach.rotation;
        circleRadius = m_checkPointToReach.localScale.magnitude;
    }

    public override void GetPlayerPosition(out Vector3 position)
    {
        position = m_playerPosition.position;
    }

    public override void GetPlayerRotation(out Quaternion rotation)
    {
        rotation = m_playerPosition.rotation;
    }

    public override void GetStageEnvironementGuid(out bool hasOne, out string stageEnvironementGuid)
    {
        hasOne = !string.IsNullOrWhiteSpace(m_stageEnvironementGuid);
        stageEnvironementGuid = m_stageEnvironementGuid;
    }

    public override void GetStageGitHubUrl(out bool hasOne, out string stageGitHubUrl)
    {
        hasOne = !string.IsNullOrWhiteSpace(m_stageEnvironementGitHubUrl);
        stageGitHubUrl = m_stageEnvironementGitHubUrl;
    }

    public override void GetStageName(out bool hasOne, out string stageName)
    {
        hasOne = !string.IsNullOrWhiteSpace(m_stageEnvironementName);
        stageName = m_stageEnvironementName;
    }

    public override void GetSurvivalTime(out float survivalTime)
    {
        survivalTime = m_currentSurvivalTime;
    }

    public override void RaycastStageEnvironement(Vector3 origin, Vector3 direction, out bool hit, out Vector3 hitPoint, float maxDistance)
    {
        hit = Physics.Raycast(origin, direction, out RaycastHit hitCallBack, maxDistance, m_stageLayerMask);
        hitPoint = hitCallBack.point;
    }

    public override void RemoveProjectileSpawnListener(I_ProjectileSpawnListener listener)
    {
        m_projectileSpawnListeners.Remove(listener);
    }

    public override void RemoveRestartLevelListener(Action listener)
    {
        m_restartLevelListeners.Remove(listener);
    }

    public override void RemoveStageChangeListener(Action listener)
    {
        m_stageChangeListeners.Remove(listener);
    }

    public override void SetJoystickLeftHorizontal(float value1to1)
    {
        if (m_userWantDefaultInput) return;
        m_joystickLeftHorizontal = value1to1;
        m_onJoystickLeftHorizontal.Invoke(value1to1);
    }

    public override void SetJoystickLeftVertical(float value1to1)
    {
        if (m_userWantDefaultInput) return;
        m_joystickLeftVertical = value1to1;
        m_onJoystickLeftVertical.Invoke(value1to1);
    }

    public override void SetJoystickRightHorizontal(float value1to1)
    {
        if (m_userWantDefaultInput) return;
        m_joystickRightHorizontal = value1to1;
        m_onJoystickRightHorizontal.Invoke(value1to1);
    }

    public override void SetJoystickRightVertical(float value1to1)
    {

        if (m_userWantDefaultInput) return;
        m_joystickRightVertical = value1to1;
        m_onJoystickRightVertical.Invoke(value1to1);
    }

    public override void GetPlayerRadius(out float sphereColliderRadius)
    {
        sphereColliderRadius = m_playerRadius.radius;
    }

    public override void ActiveDefaultInputToFlyWithoutCode(bool activeDefaultUnityInputToFly)
    {
        m_userWantDefaultInput= activeDefaultUnityInputToFly;

        if (activeDefaultUnityInputToFly)
        {
            SetJoystickLeftHorizontal(0);
            SetJoystickLeftVertical(0);
            SetJoystickRightHorizontal(0);
            SetJoystickRightVertical(0);
        }
        m_onDefaultInputEnable.Invoke(activeDefaultUnityInputToFly);
    }
    #endregion
}
