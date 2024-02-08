using Cinemachine;
using UnityEngine;

namespace World.Player
{
    public struct PlayerComp
    {
        public Transform Transform;
        public Vector3 Position;
        public Quaternion Rotation;
        public CharacterController CharacterController;
        public Transform PlayerCameraRootTransform;
        public Transform PlayerCameraStatsTransform;
        public CinemachineVirtualCamera PlayerCameraRoot;
        public CinemachineVirtualCamera PlayerCameraStats;
        public bool Grounded;
        public float VerticalVelocity;
        public bool IsWalking;
        public float CameraSense;

        public int GoldAmount;
        
        public bool CanMove;
        public bool IsPose;
    }
}