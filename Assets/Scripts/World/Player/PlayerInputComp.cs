﻿using UnityEngine;

namespace World.Player
{
    public struct PlayerInputComp
    {
        public Vector2 Move;
        public Vector2 Look;
        public float Zoom;
        public bool Jump;
        public bool Sprint;
        public bool Walk;
        public bool Dash;
        public bool FreeLook;
    }
}