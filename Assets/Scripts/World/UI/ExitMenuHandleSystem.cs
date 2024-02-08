using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Utils;
using World.Player;

namespace World.UI
{
    public class ExitMenuHandleSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerFilter = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var inputComp = ref _playerFilter.Pools.Inc2.Get(entity);

                if (inputComp.Exit)
                {
                    _sd.Value.uiSceneData.exitMenu.gameObject.SetActive(!_sd.Value.uiSceneData.exitMenu.gameObject.activeInHierarchy);
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            _sd.Value.uiSceneData.exitMenu.SetWorld(_eventWorld.Value);
        }
    }
}