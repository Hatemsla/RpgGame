using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Utils;
using World.Player;

namespace World.SaveGame
{
    public class QuickSaveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp>> _playerFilter = default;
        private readonly EcsPoolInject<SaveEventComp> _saveEventPool = Idents.Worlds.Events;
        
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _playerFilter.Value)
            {
                ref var playerInputComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);

                if (playerInputComp.QuickSave)
                {
                    var saveEventEntity = _eventWorld.Value.NewEntity();
                    _saveEventPool.Value.Add(saveEventEntity);
                }
            }
        }
    }
}