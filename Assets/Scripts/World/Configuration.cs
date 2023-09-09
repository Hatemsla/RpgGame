using Cinemachine;
using UnityEngine;

[CreateAssetMenu]
public class Configuration : ScriptableObject
{
    [Header("Player Settings")] 
    public float moveSpeed;
    public float sprintSpeed;
    public float speedChangeRate;
    public float rotationSmoothTime;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;
    public float cameraAngleOverride = 0.0f;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera playerFollowCameraPrefab;
}