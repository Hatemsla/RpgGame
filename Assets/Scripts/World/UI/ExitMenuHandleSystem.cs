using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Player;

namespace World.UI
{
    public class ExitMenuHandleSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _playerFilter = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
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
    }
}