using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
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
        private readonly EcsFilterInject<Inc<PlayerInputComp, PlayerComp, RpgComp, InventoryComp>> _player = default;

        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        private readonly EcsWorldInject _world = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var inputComp = ref _player.Pools.Inc1.Get(entity);

                if (inputComp.Inventory)
                {
                    if(!_sd.Value.uiSceneData.traderShopView.gameObject.activeInHierarchy)
                        _sd.Value.uiSceneData.playerInventoryView.gameObject.SetActive(!_sd.Value.uiSceneData.playerInventoryView.gameObject.activeInHierarchy);
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
            ref var rpgComp = ref _player.Pools.Inc3.Get(entity);
            ref var inventoryComp = ref _player.Pools.Inc4.Get(entity);
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
                            {
                                rpgComp.Health += _cf.Value.playerConfiguration.health * type.HealthPercent;
                                SpendPotion(itemIdx, inventoryComp, itemComp);
                            }

                            break;
                        case ItemManaPotion type:
                            if (unpackedEntity == itemIdx)
                            {
                                rpgComp.Mana += _cf.Value.playerConfiguration.mana * type.ManaPercent;
                                SpendPotion(itemIdx, inventoryComp, itemComp);
                            }
                            break;
                        case ItemStaminaPotion type:
                            if (unpackedEntity == itemIdx)
                            {
                                rpgComp.Stamina += _cf.Value.playerConfiguration.stamina * type.StaminaPercent;
                                SpendPotion(itemIdx, inventoryComp, itemComp);
                            }
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

        private void SpendPotion(int itemIdx, InventoryComp inventoryComp, ItemComp itemComp)
        {
            foreach (var ft in _sd.Value.fastItemViews)
            {
                if (ft.ItemIdx.Unpack(_world.Value, out var ftUnpackedEntity))
                {
                    if (ftUnpackedEntity == itemIdx)
                    {
                        Utils.Utils.ResetFastItemView(ft);
                        break;
                    }
                }
            }

            inventoryComp.CurrentWeight -= itemComp.Weight;
            inventoryComp.InventoryWeightView.inventoryWeightText.text =
                $"Вес: {inventoryComp.CurrentWeight:f1}/{inventoryComp.MaxWeight}";

            Object.Destroy(itemComp.ItemView.gameObject);

            _itemsPool.Value.Del(itemIdx);
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