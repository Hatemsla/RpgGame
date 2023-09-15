using Cinemachine;
using UnityEngine;

namespace World.Player
{
    [CreateAssetMenu(fileName = "PlayerConfiguration", menuName = "World Configurations/Player Configuration")]
    public class PlayerConfiguration : ScriptableObject
    {
        [Header("Player Settings")] 
        public float moveSpeed;
        public float sprintSpeed;
        public float walkSpeed;
        public float speedChangeRate;
        public float rotationSmoothTime;
        public float topClamp = 70.0f;
        public float bottomClamp = -30.0f;
        public float cameraAngleOverride = 0.0f;
        public float fallTimeout = 0.15f;
        public float jumpTimeout = 0.50f;
        public float zoomSpeed = 1;
        public float minZoomDistance = 0;
        public float maxZoomDistance = 15;
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;
        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;
        public GameObject playerPrefab;
        public CinemachineVirtualCamera playerFollowCameraPrefab;
    }
}