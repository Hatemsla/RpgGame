using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerJumpAndGravitySystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp, AnimationComp>> _playerMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private float _fallTimeoutDelta;
        private float _jumpTimeoutDelta;
        private float _terminalVelocity = 53.0f;
        
        private static readonly int IsJump = Animator.StringToHash("Jump");
        private static readonly int IsInAir = Animator.StringToHash("IsInAir");

        public void Init(IEcsSystems systems)
        {
            _jumpTimeoutDelta = _cf.Value.playerConfiguration.jumpTimeout;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerMove.Value)
            {
                ref var playerComp = ref _playerMove.Pools.Inc1.Get(entity);
                ref var inputComp = ref _playerMove.Pools.Inc2.Get(entity);
                ref var rpgComp = ref _playerMove.Pools.Inc3.Get(entity);
                ref var animationComp = ref _playerMove.Pools.Inc4.Get(entity);
                
                if(rpgComp.IsDead) return;
                
                var jumpEndurance = rpgComp.Stamina - _cf.Value.playerConfiguration.jumpEndurance;
                rpgComp.CanJump = jumpEndurance > 0;
                
                if (playerComp.Grounded)
                {
                    animationComp.Animator.SetBool(IsInAir, false);
                    _fallTimeoutDelta = _cf.Value.playerConfiguration.fallTimeout;

                    if (playerComp.VerticalVelocity < 0.0f)
                    {
                        playerComp.VerticalVelocity = -2f;
                    }
                    
                    if (inputComp.Jump && _jumpTimeoutDelta <= 0f)
                    {
                        if (rpgComp.CanJump)
                        {
                            rpgComp.Stamina = jumpEndurance;
                            playerComp.VerticalVelocity = Mathf.Sqrt(_cf.Value.playerConfiguration.jumpHeight * -2f *
                                                                 _cf.Value.playerConfiguration.gravity);
                            animationComp.Animator.SetTrigger(IsJump);
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
                    animationComp.Animator.SetBool(IsInAir, true);

                    if (_fallTimeoutDelta >= 0f)
                    {
                        _fallTimeoutDelta -= _ts.Value.DeltaTime;
                    }
                    else
                    {
                        // TODO: set animation
                    }
                }

                if (playerComp.VerticalVelocity < _terminalVelocity)
                {
                    playerComp.VerticalVelocity += _cf.Value.playerConfiguration.gravity * _ts.Value.DeltaTime;
                }
            }
        }
    }
}