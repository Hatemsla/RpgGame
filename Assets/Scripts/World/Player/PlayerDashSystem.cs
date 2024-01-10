using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public class PlayerDashSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp, AnimationComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        
        private static readonly int RollForward = Animator.StringToHash("RollForward");
        private static readonly int RollBackward = Animator.StringToHash("RollBackward");

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);
                ref var rpg = ref _playerMove.Pools.Inc3.Get(entity);
                ref var animationComp = ref _playerMove.Pools.Inc4.Get(entity);
                
                if(rpg.IsDead) return;
                
                var dashEndurance = rpg.Stamina - _cf.Value.playerConfiguration.dashEndurance;
                rpg.CanDash = dashEndurance > 0;
                
                if (input.Dash && player.Grounded)
                {
                    if (rpg.CanDash)
                    {
                        var dashDirection =
                            input.Move * (_cf.Value.playerConfiguration.dashSpeed * _ts.Value.DeltaTime);

                        if (dashDirection != Vector2.zero)
                        {
                            player.CharacterController.Move(new Vector3(dashDirection.x, 0f, dashDirection.y));
                            rpg.Stamina = dashEndurance;
                        }

                        Debug.Log(dashDirection);
                        
                        if(dashDirection.y > 0)
                            animationComp.Animator.SetTrigger(RollForward);
                        else if(dashDirection.y < 0)
                            animationComp.Animator.SetTrigger(RollBackward);
                    }
                }
            }
        }
    }
}