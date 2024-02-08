using UnityEngine;

namespace World.SaveGame
{
    public class PlayerData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public bool Grounded;
        public float VerticalVelocity;
        public bool IsWalking;
        public float CameraSense;
        public int GoldAmount;
        public bool CanMove;
    }
}