using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Configurations;

namespace World.Player
{
    public sealed class CursorControllingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp, PlayerComp>> _inputs = default;
        private readonly EcsCustomInject<CursorService> _cs = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly GameObject _playerInventoryView = default;

        [EcsUguiNamed(Idents.UI.ChestInventoryView)]
        private readonly GameObject _chestInventoryView = default;

        [EcsUguiNamed(Idents.UI.StatsLevelCanvas)]
        private readonly GameObject _statsLevelView = default;

        public void Init(IEcsSystems systems)
        {
            _cs.Value.CursorVisible = false;
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _inputs.Value)
            {
                ref var inputComp = ref _inputs.Pools.Inc1.Get(entity);
                ref var playerComp = ref _inputs.Pools.Inc2.Get(entity);

                _cs.Value.CursorVisible = _chestInventoryView.activeSelf || _playerInventoryView.activeSelf ||
                                          _statsLevelView.activeSelf ||
                                          _sd.Value.uiSceneData.traderShopView.gameObject.activeInHierarchy ||
                                          _sd.Value.uiSceneData.exitMenu.gameObject.activeInHierarchy ||
                                          inputComp.FreeCursor;

                Cursor.visible = _cs.Value.CursorVisible;

                Cursor.lockState = !_cs.Value.CursorVisible ? CursorLockMode.Locked : CursorLockMode.Confined;
            }
        }
    }
}