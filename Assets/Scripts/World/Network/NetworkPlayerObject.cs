using Fusion;

namespace World.Network
{
    public class NetworkPlayerObject : NetworkBehaviour, IPlayerLeft
    {
        public static NetworkPlayerObject Local { get; set; }
        
        public override void Spawned()
        {
            Utils.Utils.DebugLog($"Player spawned, has input auth {Object.HasInputAuthority}");

            if (Object.HasInputAuthority)
            {
                Local = this;
            }
            
            Runner.SetPlayerObject(Object.InputAuthority, Object);

            transform.name = $"Player_{Object.Id}";
        }
        
        public void PlayerLeft(PlayerRef player)
        {
            if (player == Object.InputAuthority)
                Runner.Despawn(Object);
        }
    }
}