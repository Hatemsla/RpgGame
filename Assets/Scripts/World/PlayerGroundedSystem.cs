using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class PlayerGroundedSystem : IEcsRunSystem
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
            
            var spherePosition = new Vector3(player.Position.x, player.Position.y - _cf.Value.groundedOffset,
                player.Position.z);
            player.Grounded = Physics.CheckSphere(spherePosition, _cf.Value.groundedRadius, _cf.Value.groundLayers,
                QueryTriggerInteraction.Ignore);
        }
    }
}