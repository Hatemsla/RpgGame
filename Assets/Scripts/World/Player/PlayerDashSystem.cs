using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;

namespace World.Player
{
    public class PlayerDashSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);
                ref var rpg = ref _playerMove.Pools.Inc3.Get(entity);
                
                if(rpg.IsDead) return;
                
                var dashEndurance = rpg.Stamina - _cf.Value.playerConfiguration.dashEndurance;
                rpg.CanDash = dashEndurance > 0;
                
                if (input.Dash && player.Grounded)
                {
                    if (rpg.CanDash)
                    {
                        rpg.Stamina = dashEndurance;
                        var dashDirection =
                            input.Move * (_cf.Value.playerConfiguration.dashSpeed * _ts.Value.DeltaTime);
                        player.CharacterController.Move(new Vector3(dashDirection.x, 0f, dashDirection.y));
                    }
                }
            }
        }
    }
}