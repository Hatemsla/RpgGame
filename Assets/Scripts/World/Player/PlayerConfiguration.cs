using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace World.Player
{
    [CreateAssetMenu(fileName = "PlayerConfiguration", menuName = "World Configurations/Player Configuration", order = 0)]
    public class PlayerConfiguration : ScriptableObject
    {
        [Header("Player Settings")] 
        [Header("Move settings")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed;
        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed;
        [Tooltip("Walk speed of the character in m/s")]
        public float walkSpeed;
        [Tooltip("Dash speed of the character in m/s")]
        public float dashSpeed = 20f;
        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate;
        [Tooltip("How fast the character turns to face movement direction")]
        public float rotationSmoothTime;
        [Tooltip("How fast the character turns to face camera direction")]
        public float rotationSpeed = 2f;
        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride = 0.0f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;
        [Tooltip("Zoom speed of the character camera")]
        public float zoomSpeed = 1;
        [Tooltip("Min value of camera zoom")]
        public float minZoomDistance;
        [Tooltip("Max value of camera zoom")]
        public float maxZoomDistance = 15;
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;
        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [Header("Rpg settings")] 
        [Tooltip("Start player health points")]
        public float health = 100;
        [Tooltip("Start player stamina points")]
        public float stamina = 100;
        [Tooltip("Start player mana points")]
        public float mana = 100;
        
        [Tooltip("Endurance consumption for Sprint")]
        public float sprintEndurance;
        [Tooltip("Endurance consumption for Dash")]
        public float dashEndurance;
        [Tooltip("Endurance consumption for Jump")]
        public float jumpEndurance;
        [Tooltip("Stamina recovery rate")]
        public float staminaRecovery;

        [Tooltip("The degree of loss of health from falling from a great height")]
        public float fallDamage;
        [Tooltip("Min height of taking damage")]
        public float minDamageHeight = -10;
        [Tooltip("Health recovery rate")] 
        public float healthRecovery;

        [Tooltip("Mana recovery rate")]
        public float manaRecovery;
        
        [Tooltip("Start level")]
        public int startLevel = 1;
        [Tooltip("Start experience")]
        public int startExperience;
        [Tooltip("Experience needed to reach next level")]
        public List<int> experienceToNextLevel;
        
        [Header("Other settings")]
        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;
        [Tooltip("Character prefab to instantiate")]
        public GameObject playerPrefab;
        [Tooltip("Character follow camera prefab to instantiate")]
        public CinemachineVirtualCamera playerFollowCameraPrefab;
    }
}