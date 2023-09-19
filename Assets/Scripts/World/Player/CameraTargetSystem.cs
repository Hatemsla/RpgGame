using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace World.Player
{
    public class CameraTargetSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerInputs = default;
        
        
        
        public void Run(IEcsSystems systems)
        {
                
        }
    }
}