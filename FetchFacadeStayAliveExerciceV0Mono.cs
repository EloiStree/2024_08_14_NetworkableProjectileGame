using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FetchFacadeStayAliveExerciceV0Mono : A_FacadeStayAliveExerciceMono
{


    public string m_droneName = "Tello Root of Knowledge";
    public string m_droneCodeId = "RootOfKnowledge_2024_08_15";
    public string m_droneGitCodeUrl = "https://github.com/EloiStree/2023_02_19_KidToyDroneTelloModeCode";

    [Range(-1f,1f)]
    public float m_joystickLeftHorizontal;
    [Range(-1f, 1f)]
    public float m_joystickLeftVertical;
    [Range(-1f, 1f)] 
    public float m_joystickRightHorizontal;
    [Range(-1f, 1f)] 
    public float m_joystickRightVertical;
    public UnityEvent m_onJoystickLeftHorizontal;
    public UnityEvent m_onJoystickLeftVertical;
    public UnityEvent m_onJoystickRightHorizontal;
    public UnityEvent m_onJoystickRightVertical;

    public SNAM16K_ObjectVector3 m_projectilsPosition;
    public Transform m_playerPosition;

    public LayerMask m_stageLayerMask;



    #region ABSTRACT TO IMPLEMENT
    public override void AddProjectileSpawnListener(I_ProjectileSpawnListener listener)
    {
        throw new NotImplementedException();
    }

    public override void AddRestartLevelListener(Action listener)
    {
        throw new NotImplementedException();
    }

    public override void GetAllProjectilesPositions(out NativeArray<Vector3> positions)
    {
        throw new NotImplementedException();
    }

    public override void GetDroneCodeBehaviourGuid(out string droneCodeId)
    {
        throw new NotImplementedException();
    }

    public override void GetDroneProductName(out string droneProductName)
    {
        throw new NotImplementedException();
    }

    public override void GetJoystickLeftHorizontal(out float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void GetJoystickLeftVertical(out float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void GetJoystickRightHorizontal(out float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void GetJoystickRightVertical(out float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void GetPlayerPosition(out Vector3 position)
    {
        throw new NotImplementedException();
    }

    public override void GetPlayerRotation(out Quaternion rotation)
    {
        throw new NotImplementedException();
    }

    public override void GetSurvivalTime(out float survivalTime)
    {
        throw new NotImplementedException();
    }

    public override void RaycastStageEnvironement(Vector3 origin, Vector3 direction, out bool hit, out Vector3 hitPoint, float maxDistance)
    {
        hit = Physics.Raycast(origin, direction, out RaycastHit hitCallBack, maxDistance, m_stageLayerMask);
        hitPoint = hitCallBack.point;
    }

    public override void RemoveProjectileSpawnListener(I_ProjectileSpawnListener listener)
    {
        throw new NotImplementedException();
    }

    public override void RemoveRestartLevelListener(Action listener)
    {
        throw new NotImplementedException();
    }

    public override void SetJoystickLeftHorizontal(float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void SetJoystickLeftVertical(float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void SetJoystickRightHorizontal(float value1to1)
    {
        throw new NotImplementedException();
    }

    public override void SetJoystickRightVertical(float value1to1)
    {
        throw new NotImplementedException();
    }
    #endregion
}
