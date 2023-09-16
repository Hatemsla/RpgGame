using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public class PlayerDashSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);

                if (input.Dash && player.Grounded)
                {
                    var dashDirection = input.Move * (_cf.Value.playerConfiguration.dashSpeed * _ts.Value.DeltaTime);
                    player.CharacterController.Move(new Vector3(dashDirection.x, 0f, dashDirection.y));
                }
            }
        }
    }
}