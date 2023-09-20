using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

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
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var rpg = ref _player.Pools.Inc2.Get(entity);

                if (!player.Grounded)
                {
                    if (player.VerticalVelocity < _cf.Value.playerConfiguration.minDamageHeight)
                    {
                        _isLargeHeight = true;
                        if (_fellDamage > player.VerticalVelocity)
                            _fellDamage = player.VerticalVelocity;
                    }
                }
                else
                {
                    if (_isLargeHeight)
                    {
                        rpg.Health += (_fellDamage - _cf.Value.playerConfiguration.minDamageHeight) * _cf.Value.playerConfiguration.fallDamage;
                        _isLargeHeight = false;
                    }
                }
            }
        }
    }
}