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
                ref var inputComp = ref _player.Pools.Inc1.Get(entity);
                ref var playerComp = ref _player.Pools.Inc2.Get(entity);
                ref var rpgComp = ref _player.Pools.Inc3.Get(entity);

                if (inputComp.Inventory)
                {
                    _inventoryView.SetActive(!_inventoryView.activeSelf);
                }

                if (inputComp.Alpha1)
                {
                    if (_sd.Value.fastItemViews[0].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (inputComp.Alpha2)
                {
                    if (_sd.Value.fastItemViews[1].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (inputComp.Alpha3)
                {
                    if (_sd.Value.fastItemViews[2].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (inputComp.Alpha4)
                {
                    if (_sd.Value.fastItemViews[3].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (inputComp.Alpha5)
                {
                    if (_sd.Value.fastItemViews[4].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }

                if (inputComp.Alpha6)
                {
                    if (_sd.Value.fastItemViews[5].ItemIdx.Unpack(_world.Value, out var unpackedItem))
                        TryGetItem(unpackedItem, entity);
                }
            }
        }

        private void TryGetItem(int itemIdx, int entity)
        {
            ref var hasItems = ref _hasItemsPool.Value.Get(entity);
            ref var rpg = ref _player.Pools.Inc3.Get(entity);
            ref var getItem = ref _itemsPool.Value.Get(itemIdx);

            foreach (var itemPacked in hasItems.Entities)
            {
                if (itemPacked.Unpack(_world.Value, out var unpackedEntity))
                {
                    ref var itemComp = ref _itemsPool.Value.Get(unpackedEntity);
                    switch (itemComp.ItemType)
                    {
                        // Potions
                        case ItemHealthPotion type:
                            if (unpackedEntity == itemIdx)
                                rpg.Health += _cf.Value.playerConfiguration.health * type.HealthPercent;
                            break;
                        case ItemManaPotion type:
                            if (unpackedEntity == itemIdx)
                                rpg.Mana += _cf.Value.playerConfiguration.mana * type.ManaPercent;
                            break;
                        case ItemStaminaPotion type:
                            if (unpackedEntity == itemIdx)
                                rpg.Stamina += _cf.Value.playerConfiguration.stamina * type.StaminaPercent;
                            break;
                        // Weapons
                        case ItemShieldWeapon:
                            ToggleItemObject(itemComp, getItem, unpackedEntity, itemIdx);
                            break;
                        case ItemSwordWeapon:
                            ToggleItemObject(itemComp, getItem, unpackedEntity, itemIdx);
                            break;
                        case ItemBowWeapon:
                            ToggleItemObject(itemComp, getItem, unpackedEntity, itemIdx);
                            break;
                        // Tools
                        case ItemTool:
                            ToggleItemObject(itemComp, getItem, unpackedEntity, itemIdx);
                            break;
                    }
                }
            }
        }

        private void ToggleItemObject(ItemComp item, ItemComp getItem, int unpackedEntity, int itemIdx)
        {
            if (unpackedEntity == itemIdx)
                item.ItemView.itemObject.gameObject.SetActive(!item.ItemView.itemObject.gameObject.activeSelf);
            else if(getItem.ItemType is not ItemPotion)
                item.ItemView.itemObject.gameObject.SetActive(false);
        }
    }
}