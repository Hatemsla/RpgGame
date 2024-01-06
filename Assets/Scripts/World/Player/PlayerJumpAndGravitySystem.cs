using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerJumpAndGravitySystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private float _fallTimeoutDelta;
        private float _jumpTimeoutDelta;
        private float _terminalVelocity = 53.0f;

        public void Init(IEcsSystems systems)
        {
            _jumpTimeoutDelta = _cf.Value.playerConfiguration.jumpTimeout;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var player = ref _playerMove.Pools.Inc1.Get(entity);
                ref var input = ref _playerMove.Pools.Inc2.Get(entity);
                ref var rpg = ref _playerMove.Pools.Inc3.Get(entity);
                
                if(rpg.IsDead) return;
                
                var jumpEndurance = rpg.Stamina - _cf.Value.playerConfiguration.jumpEndurance;
                rpg.CanJump = jumpEndurance > 0;
                
                if (player.Grounded)
                {
                    _fallTimeoutDelta = _cf.Value.playerConfiguration.fallTimeout;

                    if (player.VerticalVelocity < 0.0f)
                    {
                        player.VerticalVelocity = -2f;
                    }
                    
                    if (input.Jump && _jumpTimeoutDelta <= 0f)
                    {
                        if (rpg.CanJump)
                        {
                            rpg.Stamina = jumpEndurance;
                            player.VerticalVelocity = Mathf.Sqrt(_cf.Value.playerConfiguration.jumpHeight * -2f *
                                                                 _cf.Value.playerConfiguration.gravity);
                        }
                    }

                    if (_jumpTimeoutDelta >= 0.0f)
                    {
                        _jumpTimeoutDelta -= _ts.Value.DeltaTime;
                    }
                }
                else
                {
                    _jumpTimeoutDelta = _cf.Value.playerConfiguration.jumpTimeout;

                    if (_fallTimeoutDelta >= 0f)
                    {
                        _fallTimeoutDelta -= _ts.Value.DeltaTime;
                    }
                    else
                    {
                        // TODO: set animation
                    }
                }

                if (player.VerticalVelocity < _terminalVelocity)
                {
                    player.VerticalVelocity += _cf.Value.playerConfiguration.gravity * _ts.Value.DeltaTime;
                }
            }
        }
    }
}