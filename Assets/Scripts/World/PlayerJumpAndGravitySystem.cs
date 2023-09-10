using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class PlayerJumpAndGravitySystem : IEcsRunSystem, IEcsInitSystem
{
    private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerMove = default;
    
    private readonly EcsCustomInject<Configuration> _cf = default;
    private readonly EcsCustomInject<TimeService> _ts = default;
    
    private float _fallTimeoutDelta;
    private float _jumpTimeoutDelta;
    private float _terminalVelocity = 53.0f;

    public void Init(IEcsSystems systems)
    {
        _jumpTimeoutDelta = _cf.Value.jumpTimeout;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _playerMove.Value)
        {
            ref var player = ref _playerMove.Pools.Inc1.Get(entity);
            ref var input = ref _playerMove.Pools.Inc2.Get(entity);

            if (player.Grounded)
            {
                _fallTimeoutDelta = _cf.Value.fallTimeout;

                if (player.VerticalVelocity < 0.0f)
                {
                    player.VerticalVelocity = -2f;
                }

                if (input.Jump && _jumpTimeoutDelta <= 0f)
                {
                    player.VerticalVelocity = Mathf.Sqrt(_cf.Value.jumpHeight * -2f * _cf.Value.gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= _ts.Value.DeltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = _cf.Value.jumpTimeout;
                
                if (_fallTimeoutDelta >= 0f)
                {
                    _fallTimeoutDelta -= _ts.Value.DeltaTime;
                }
                else
                {
                    // TODO: set animation
                }

                input.Jump = false;
            }
            
            if (player.VerticalVelocity < _terminalVelocity)
            {
                player.VerticalVelocity += _cf.Value.gravity * _ts.Value.DeltaTime;
            }
        }
    }
}