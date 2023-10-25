using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Player;

namespace World.Inventory.Chest
{
    public sealed class ChestUpdateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _player = default;
        
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var chest in _sd.Value.chests)
            {
                foreach (var entity in _player.Value)
                {
                    ref var input = ref _player.Pools.Inc2.Get(entity);

                    chest.isOpen = input.ActiveAction;
                }
            }
        }
    }
}