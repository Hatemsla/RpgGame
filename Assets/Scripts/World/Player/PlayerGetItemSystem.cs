using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Inventory;
using World.Inventory.ItemTypes;
using World.Inventory.ItemTypes.Potions;
using World.Inventory.ItemTypes.Weapons;
using World.RPG;

namespace World.Player
{
    public class PlayerGetItemSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerInputComp, PlayerComp, RpgComp>> _player = default;
        
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        private readonly EcsWorldInject _world = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
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
                
                if (input.Alpha1)
                {
                    if(_sd.Value.fastItemViews[0].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (input.Alpha2)
                {
                    if(_sd.Value.fastItemViews[1].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }
                
                if (input.Alpha3)
                {
                    if(_sd.Value.fastItemViews[2].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }
                
                if (input.Alpha4)
                {
                    if(_sd.Value.fastItemViews[3].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (input.Alpha5)
                {
                    if(_sd.Value.fastItemViews[4].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }
                
                if (input.Alpha6)
                {
                    if(_sd.Value.fastItemViews[5].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }
            }
        }

        private void TryGetItem(int itemIdx, int entity)
        {
            var world = _hasItemsPool.Value.GetWorld();
            ref var hasItems = ref _hasItemsPool.Value.Get(entity);
            ref var rpg = ref _player.Pools.Inc3.Get(entity);

            foreach (var itemPacked in hasItems.Entities)
            {
                if (itemPacked.Unpack(world, out var unpackedEntity))
                {
                    ref var item = ref _itemsPool.Value.Get(unpackedEntity);
                    switch (item.ItemType)
                    {
                        // Potions
                        case ItemHealthPotion type:
                            if (unpackedEntity == itemIdx)
                                rpg.Health += _cf.Value.playerConfiguration.health * type.healthPercent;
                            break;
                        case ItemManaPotion type:
                            if (unpackedEntity == itemIdx)
                                rpg.Mana += _cf.Value.playerConfiguration.mana * type.manaPercent;
                            break;
                        // Weapons
                        case ItemShieldWeapon:
                            CreateItemView(item, unpackedEntity, itemIdx);
                            break;
                        case ItemSwordWeapon:
                            CreateItemView(item, unpackedEntity, itemIdx);
                            break;
                        case ItemBowWeapon:
                            CreateItemView(item, unpackedEntity, itemIdx);
                            break;
                        // Tools
                        case ItemTool:
                            CreateItemView(item, unpackedEntity, itemIdx);
                            break;
                    }
                }
            }
        }

        private void CreateItemView(ItemComp item, int unpackedEntity,int itemIdx)
        {
            if (unpackedEntity == itemIdx)
                item.ItemView.itemObject.gameObject.SetActive(!item.ItemView.itemObject.gameObject.activeSelf);
            else
                item.ItemView.itemObject.gameObject.SetActive(false);
        }
    }
}