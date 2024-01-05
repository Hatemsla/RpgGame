using Fusion;
using UnityEngine;

namespace World.Player
{
    public struct PlayerInputComp : INetworkInput
    {
        public Vector2 Move;
        public Vector2 Look;
        public float Zoom;
        public NetworkBool Jump;
        public NetworkBool Sprint;
        public NetworkBool Walk;
        public NetworkBool Dash;
        public NetworkBool FreeLook;
        public NetworkBool Alpha1;
        public NetworkBool Alpha2;
        public NetworkBool Alpha3;
        public NetworkBool Alpha4;
        public NetworkBool Alpha5;
        public NetworkBool Alpha6;
        public NetworkBool UseAbility;
        public NetworkBool Inventory;
        public NetworkBool AutoRun;
        public NetworkBool FreeCursor;
        public NetworkBool ActiveAction;
    }
}