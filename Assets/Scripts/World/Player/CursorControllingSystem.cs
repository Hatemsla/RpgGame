using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;

namespace World.Player
{
    public sealed class CursorControllingSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp>> _inputs = default;
        private readonly EcsCustomInject<CursorService> _cs = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly GameObject _inventoryView = default;
        
        public void Init(IEcsSystems systems)
        {
            _cs.Value.CursorVisible = false;
        }
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _inputs.Value)
            {
                ref var input = ref _inputs.Pools.Inc1.Get(entity);

                _cs.Value.CursorVisible = _inventoryView.activeSelf || input.FreeCursor;

                Cursor.visible = _cs.Value.CursorVisible;
            }
        }
    }
}