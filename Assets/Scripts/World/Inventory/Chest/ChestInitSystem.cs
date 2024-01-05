using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Network;

namespace World.Inventory.Chest
{
    public sealed class ChestInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<ChestComp> _chestPool = default;
        private readonly EcsPoolInject<ItemComp> _itemPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;

        private readonly EcsWorldInject _itemsWorld = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<NetworkRunnerService> _nrs = default;
        
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
            if(!_nrs.Value.isPlayerJoined)
                return;
            
            var world = _itemsWorld.Value;
            
            foreach (var chest in _sd.Value.chests)
            {
                var entity = world.NewEntity();
                ref var chestComp = ref _chestPool.Value.Add(entity);
                ref var inventoryComp = ref _inventoryPool.Value.Add(entity);
                ref var hasItemsComp = ref _hasItemsPool.Value.Add(entity);

                chestComp.ChestObject = chest;
                chestComp.ChestObject.SetView(_chestInventoryView);
                chestComp.ChestObject.chestInventoryWeightText = _chestInventoryWeightText.GetComponent<InventoryWeightView>().inventoryWeightText;
                chestComp.ChestObject.SetWorld(world, entity);

                foreach (var itemData in chest.items)
                {
                    var itemEntity = world.NewEntity();
                    var itemPackedEntity = world.PackEntity(itemEntity);
                    ref var it = ref _itemPool.Value.Add(itemEntity);
                    
                    it.ItemName = itemData.itemName;
                    it.ItemDescription = itemData.itemDescription;
                    it.Cost = itemData.cost;
                    it.Weight = itemData.itemWeight;
                    
                    var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                        Vector3.zero,
                        itemData.itemObjectPrefab.transform.rotation);
                    itemObject.transform.SetParent(chest.transform);
                    itemObject.gameObject.SetActive(false);
                    
                    var itemView = Object.Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                    itemView.transform.SetParent(chest.transform);
                
                    itemView.itemImage.sprite = itemData.itemSprite;

                    inventoryComp.MaxWeight = _cf.Value.chestConfiguration.chestInventoryMaxWeight;
                    inventoryComp.CurrentWeight += itemData.itemWeight;
                    inventoryComp.InventoryWeightView = _chestInventoryWeightText.GetComponent<InventoryWeightView>();
                    
                    chest.itemViews.Add(itemView);
                    it.ItemView = itemView;
                    it.ItemView.itemObject = itemObject;
                    it.ItemView.itemObject.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemName = itemData.itemName;
                    it.ItemView.ItemDescription = itemData.itemDescription;
                    it.ItemView.ItemCount = itemData.itemCount.ToString();
                    it.ItemView.SetWorld(world, entity, _sd.Value);
                    it.ItemView.SetViews(_playerInventoryView, _chestInventoryView, _fastItemsView, _deleteFormView, _crosshairView);

                    hasItemsComp.Entities.Add(itemPackedEntity);
                }
            }
            
            _chestInventoryView.gameObject.SetActive(false);
        }
    }
}