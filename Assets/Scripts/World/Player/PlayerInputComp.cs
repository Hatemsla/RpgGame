using UnityEngine;

namespace World.Player
{
    public struct PlayerInputComp
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool Jump;
        public bool Sprint;
        public bool Walk;
        public float Zoom;
    }
}