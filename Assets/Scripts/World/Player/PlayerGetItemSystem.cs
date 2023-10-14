using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Inventory;

namespace World.Player
{
    public class PlayerGetItemSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp, PlayerComp>> _player = default;
        
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        
        [EcsUguiNamed(Idents.UI.InventoryView)]
        private readonly GameObject _inventoryView = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var input = ref _player.Pools.Inc1.Get(entity);
                ref var player = ref _player.Pools.Inc2.Get(entity);

                if (input.Inventory)
                {
                    _inventoryView.SetActive(!_inventoryView.activeSelf);
                }
                
                if (input.GetFirstItem)
                {
                    TryGetItem(0, entity);
                }

                if (input.GetSecondItem)
                {
                    TryGetItem(1, entity);
                }
                
                if (input.GetThirdItem)
                {
                    TryGetItem(2, entity);
                }
            }
        }

        private void TryGetItem(int itemIdx, int entity)
        {
            var world = _hasItemsPool.Value.GetWorld();
            ref var hasItems = ref _hasItemsPool.Value.Get(entity);

            for (var i = 0; i < hasItems.Entities.Count; i++)
            {
                var itemPacked = hasItems.Entities[i];

                if (itemPacked.Unpack(world, out var unpackedEntity))
                {
                    ref var item = ref _itemsPool.Value.Get(unpackedEntity);

                    if (i == itemIdx)
                    {
                        item.ItemObject.gameObject.SetActive(!item.ItemObject.gameObject.activeSelf);
                    }
                    else
                    {
                        item.ItemObject.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}