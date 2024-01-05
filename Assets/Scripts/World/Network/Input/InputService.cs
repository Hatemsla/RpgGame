using UnityEngine;
using World.Player;

namespace World.Network.Input
{
    public sealed class InputService
    {
        public PlayerInputComp playerInputComp { get; private set; }
        
        public void GetNetworkInput(PlayerInputComp playerInputComp) => this.playerInputComp = playerInputComp;
    }
}