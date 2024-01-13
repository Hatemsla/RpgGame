using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using Utils;
using World.Configurations;

namespace World.Inventory.Chest
{
    public sealed class ChestInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<ChestComp> _chestPool = default;
        private readonly EcsPoolInject<ItemComp> _itemPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;

        private readonly EcsWorldInject _itemsWorld = default;
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly RectTransform _playerInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryView)]
        private readonly RectTransform _chestInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.FastItemsView)]
        private readonly RectTransform _fastItemsView = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryWeight)]
        private readonly Transform _chestInventoryWeightText = default;
        
        [EcsUguiNamed(Idents.UI.DeleteFormView)]
        private readonly RectTransform _deleteFormView = default;
        
        [EcsUguiNamed(Idents.UI.CrosshairView)]
        private readonly RectTransform _crosshairView = default;

        public void Init(IEcsSystems systems)
        {
            foreach (var chest in _sd.Value.chests)
            {
                var entity = _itemsWorld.Value.NewEntity();
                ref var chestComp = ref _chestPool.Value.Add(entity);
                ref var inventoryComp = ref _inventoryPool.Value.Add(entity);
                ref var hasItemsComp = ref _hasItemsPool.Value.Add(entity);

                chestComp.ChestObject = chest;
                chestComp.ChestObject.SetView(_chestInventoryView);
                chestComp.ChestObject.chestInventoryWeightText = _chestInventoryWeightText.GetComponent<InventoryWeightView>().inventoryWeightText;
                chestComp.ChestObject.SetWorld(_itemsWorld.Value, entity);

                foreach (var itemData in chest.items)
                {
                    var itemEntity = _itemsWorld.Value.NewEntity();
                    var itemPackedEntity = _itemsWorld.Value.PackEntity(itemEntity);
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
                    inventoryComp.InventoryWeightView = _chestInventoryWeightText.GetComponent<InventoryWeightView>();
                    
                    chest.itemViews.Add(itemView);
                    it.ItemView = itemView;
                    it.ItemView.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemName = itemData.itemName;
                    it.ItemView.ItemDescription = itemData.itemDescription;
                    it.ItemView.ItemCount = itemData.itemCount.ToString();
                    it.ItemView.SetWorld(_itemsWorld.Value, _eventWorld.Value, entity, _sd.Value);
                    it.ItemView.SetViews(_playerInventoryView, _chestInventoryView, _fastItemsView, _deleteFormView, _crosshairView);
                    
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
            
            _chestInventoryView.gameObject.SetActive(false);
        }
    }
}