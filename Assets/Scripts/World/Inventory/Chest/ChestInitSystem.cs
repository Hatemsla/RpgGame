using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Configurations;
using World.LoadGame;

namespace World.Inventory.Chest
{
    public sealed class ChestInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<LoadDataEventComp>> _loadDataFilter = Idents.Worlds.Events;
        private readonly EcsPoolInject<ChestComp> _chestPool = default;
        private readonly EcsPoolInject<ItemComp> _itemPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;

        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Init(IEcsSystems systems)
        {
            foreach (var loadDataEntity in _loadDataFilter.Value)
            {
                ref var loadDataComp = ref _loadDataFilter.Pools.Inc1.Get(loadDataEntity);
                
                var chestIndex = 0;
                foreach (var chest in _sd.Value.chests)
                {
                    var entity = _defaultWorld.Value.NewEntity();
                    ref var chestComp = ref _chestPool.Value.Add(entity);
                    ref var inventoryComp = ref _inventoryPool.Value.Add(entity);
                    ref var hasItemsComp = ref _hasItemsPool.Value.Add(entity);

                    if (loadDataComp.IsLoadData)
                    {
                        var allItems = _cf.Value.inventoryConfiguration.allItems;
                        chest.items.Clear();
                        var chestSaveData = loadDataComp.ChestSaveDatas.ChestDatas[chestIndex];
                        foreach (var itemData in allItems)
                        {
                            foreach (var loadItem in chestSaveData.ItemDatas.Items)
                            {
                                if (itemData.itemName == loadItem.ItemName)
                                {
                                    chest.items.Add(itemData);           
                                }
                            }
                        }
                        chestIndex++;
                    }

                    chestComp.ChestObject = chest;
                    chestComp.ChestObject.SetView(_sd.Value.uiSceneData.chestInventoryView);
                    chestComp.ChestObject.chestInventoryWeightText =
                        _sd.Value.uiSceneData.chestInventoryWeightText.inventoryWeightText;
                    chestComp.ChestObject.SetWorld(_defaultWorld.Value, entity);

                    foreach (var itemData in chest.items)
                    {
                        var itemEntity = _defaultWorld.Value.NewEntity();
                        var itemPackedEntity = _defaultWorld.Value.PackEntity(itemEntity);
                        ref var it = ref _itemPool.Value.Add(itemEntity);

                        it.ItemName = itemData.itemName;
                        it.ItemDescription = itemData.itemDescription;
                        it.Cost = itemData.cost;
                        it.Weight = itemData.itemWeight;

                        var itemView = Object.Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                        itemView.transform.SetParent(chest.transform);

                        itemView.itemImage.sprite = itemData.itemSprite;

                        inventoryComp.MaxWeight = _cf.Value.chestConfiguration.chestInventoryMaxWeight;
                        inventoryComp.CurrentWeight += itemData.itemWeight;
                        inventoryComp.InventoryWeightView = _sd.Value.uiSceneData.chestInventoryWeightText;

                        chest.itemViews.Add(itemView);
                        it.ItemView = itemView;
                        it.ItemView.ItemIdx = itemPackedEntity;
                        it.ItemView.ItemName = itemData.itemName;
                        it.ItemView.ItemDescription = itemData.itemDescription;
                        it.ItemView.ItemCount = itemData.itemCount.ToString();
                        it.ItemView.SetWorld(_defaultWorld.Value, _eventWorld.Value, entity, _sd.Value);
                        it.ItemView.SetViews(_sd.Value.uiSceneData.playerInventoryView,
                            _sd.Value.uiSceneData.chestInventoryView, _sd.Value.uiSceneData.fastItemsView,
                            _sd.Value.uiSceneData.deleteFormView, _sd.Value.uiSceneData.crosshairView);

                        if (itemData.itemObjectPrefab)
                        {
                            var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                                Vector3.zero,
                                itemData.itemObjectPrefab.transform.rotation);
                            itemObject.transform.SetParent(chest.transform);
                            itemObject.gameObject.SetActive(false);
                            it.ItemView.itemObject = itemObject;
                            it.ItemView.itemObject.ItemIdx = itemPackedEntity;
                        }

                        hasItemsComp.Entities.Add(itemPackedEntity);
                    }
                }

                _sd.Value.uiSceneData.chestInventoryView.gameObject.SetActive(false);
            }
        }
    }
}