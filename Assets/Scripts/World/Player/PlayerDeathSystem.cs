using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Player;
using World.RPG;

namespace World.Player
{
    public class PlayerDeathSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _player = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var rpg = ref _player.Pools.Inc2.Get(entity);

                if (rpg.IsDead)
                {
                    if (rpg.Health > 0)
                        rpg.IsDead = false;
                }
                else
                {
                    if (rpg.Health < 0)
                        rpg.IsDead = true;
                }
            }
        }
    }
}