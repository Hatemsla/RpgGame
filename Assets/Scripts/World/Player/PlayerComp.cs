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
        public Transform PlayerCameraRoot;
        public CinemachineVirtualCamera PlayerCamera;
        public bool Grounded;
        public float VerticalVelocity;
        public bool IsWalking;
        public bool IsAutoRun;
    }
}