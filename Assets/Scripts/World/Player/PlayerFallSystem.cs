using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public class PlayerFallSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _player = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private bool _isLargeHeight;
        private float _fellDamage;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var playerComp = ref _player.Pools.Inc1.Get(entity);
                ref var rpg = ref _player.Pools.Inc2.Get(entity);

                if (!playerComp.Grounded)
                {
                    if (Physics.Raycast(playerComp.Position, Vector3.down, out var hit, Mathf.Infinity))
                    {
                        var currentHeight = hit.distance;

                        if (currentHeight > _cf.Value.playerConfiguration.minDamageHeight)
                        {
                            _isLargeHeight = true;
                            if (_fellDamage < currentHeight)
                                _fellDamage = currentHeight;
                        }
                    }
                }
                else
                {
                    if (_isLargeHeight)
                    {
                        rpg.Health -= (_fellDamage - _cf.Value.playerConfiguration.minDamageHeight) * _cf.Value.playerConfiguration.fallDamage;
                        _isLargeHeight = false;
                    }
                }
            }
        }
    }
}