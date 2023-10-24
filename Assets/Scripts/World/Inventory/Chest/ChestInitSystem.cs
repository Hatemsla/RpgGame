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

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly RectTransform _playerInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryView)]
        private readonly RectTransform _chestInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryWeight)]
        private readonly TMP_Text _chestInventoryWeightText = default;
        
        public void Init(IEcsSystems systems)
        {
            var world = _itemsWorld.Value;
            
            foreach (var chest in _sd.Value.chests)
            {
                var entity = world.NewEntity();
                ref var chestComp = ref _chestPool.Value.Add(entity);
                ref var inventoryComp = ref _inventoryPool.Value.Add(entity);
                ref var hasItemsComp = ref _hasItemsPool.Value.Add(entity);

                chestComp.ChestObject = chest;

                foreach (var itemData in chest.items)
                {
                    var itemEntity = world.NewEntity();
                    var itemPackedEntity = world.PackEntity(itemEntity);
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
                    
                    it.ItemView = itemView;
                    it.ItemView.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemName = itemData.itemName;
                    it.ItemView.ItemDescription = itemData.itemDescription;
                    it.ItemView.ItemCount = itemData.itemCount.ToString();
                    it.ItemView.SetWorld(world, entity);
                    it.ItemView.playerInventoryView = _playerInventoryView;
                    it.ItemView.chestInventoryView = _chestInventoryView;
                    it.ItemView.inventoryWeightText = _chestInventoryWeightText;

                    hasItemsComp.Entities.Add(itemPackedEntity);
                }
            }
            
            _chestInventoryView.gameObject.SetActive(false);
        }
    }
}