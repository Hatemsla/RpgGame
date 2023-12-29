using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Inventory.ItemTypes;
using World.Inventory.ItemTypes.Potions;
using World.Inventory.ItemTypes.Weapons;
using World.Inventory.ItemTypesData;
using World.Inventory.ItemTypesData.PotionsData;
using World.Inventory.ItemTypesData.WeaponsData;
using World.Player;

namespace World.Inventory
{
    public class ItemsInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, InventoryComp>> _playerFilter = default;
        
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private readonly EcsWorldInject _world = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly RectTransform _playerInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryWeight)]
        private readonly Transform _playerInventoryWeightText = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryView)]
        private readonly RectTransform _chestInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.FastItemsView)]
        private readonly RectTransform _fastItemsView = default;
        
        [EcsUguiNamed(Idents.UI.DeleteFormView)]
        private readonly RectTransform _deleteFormView = default;
        
        [EcsUguiNamed(Idents.UI.CrosshairView)]
        private readonly RectTransform _crosshairView = default;
        
        public void Init(IEcsSystems systems)
        {
            _deleteFormView.gameObject.SetActive(false);
            
            foreach (var entity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);
                ref var inventoryComp = ref _playerFilter.Pools.Inc2.Get(entity);
                
                inventoryComp.MaxWeight = _cf.Value.inventoryConfiguration.inventoryWeight;
                inventoryComp.CurrentWeight = 0f;
            
                _playerInventoryView.gameObject.SetActive(false);
                
                ref var hasItems = ref _hasItemsPool.Value.Add(entity);
                var items = _cf.Value.inventoryConfiguration.items;
                var playerInventoryViewContent = _playerInventoryView.GetComponentInChildren<ContentView>();
                playerInventoryViewContent.currentEntity = entity;

                var weight = 0f;
                for (var i = 0; i < items.Count; i++)
                {
                    var itemData = items[i];
                    var itemEntity = _world.Value.NewEntity();
                    var itemPackedEntity = _world.Value.PackEntity(itemEntity);
                    ref var it = ref _itemsPool.Value.Add(itemEntity);

                    it.ItemName = itemData.itemName;
                    it.ItemDescription = itemData.itemDescription;
                    it.Cost = itemData.cost;
                    it.Weight = itemData.itemWeight;
                    it.ItemType = DefineItemType(itemData.itemTypeData);

                    weight += itemData.itemWeight;

                    var itemView = Object.Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                    itemView.transform.SetParent(playerInventoryViewContent.transform);
                    it.ItemView = itemView;

                    it.ItemView.itemImage.sprite = itemData.itemSprite;
                    it.ItemView.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemName = itemData.itemName;
                    it.ItemView.ItemDescription = itemData.itemDescription;
                    it.ItemView.ItemCount = itemData.itemCount.ToString();
                    it.ItemView.SetWorld(_world.Value, entity, _sd.Value);

                    it.ItemView.SetViews(_playerInventoryView, _chestInventoryView, _fastItemsView, _deleteFormView, _crosshairView);
                    
                    if (itemData.itemObjectPrefab)
                    {
                        var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                            playerComp.Transform.position + playerComp.Transform.forward,
                            itemData.itemObjectPrefab.transform.rotation);
                        itemObject.transform.SetParent(playerComp.Transform);
                        itemObject.gameObject.SetActive(false);
                        
                        it.ItemView.itemObject = itemObject;
                        it.ItemView.itemObject.ItemIdx = itemPackedEntity;
                        _sd.Value.fastItemViews[i].itemObject = itemObject;
                        _sd.Value.fastItemViews[i].itemObject.ItemIdx = itemPackedEntity;
                    }

                    it.ItemView.SetViews(_playerInventoryView, _chestInventoryView, _fastItemsView, _deleteFormView, _crosshairView);

                    _sd.Value.fastItemViews[i].ItemIdx = itemPackedEntity;
                    _sd.Value.fastItemViews[i].itemImage.sprite = itemData.itemSprite;
                    _sd.Value.fastItemViews[i].itemName.text = itemData.itemName;
                    _sd.Value.fastItemViews[i].itemCount.text = itemData.itemCount.ToString();

                    hasItems.Entities.Add(itemPackedEntity);
                }

                inventoryComp.CurrentWeight = weight;

                inventoryComp.InventoryWeightView = _playerInventoryWeightText.GetComponent<InventoryWeightView>();
                inventoryComp.InventoryWeightView.inventoryWeightText.text = $"Вес: {inventoryComp.CurrentWeight}/{inventoryComp.MaxWeight}";
            }
        }

        public ItemType DefineItemType(ItemTypeData itemTypeData)
        {
            ItemType value = null;
            switch (itemTypeData)
            {
                // Potions
                case HealthPotionItemData data:
                    value = new ItemHealthPotion();
                    ((ItemHealthPotion) value).healthPercent = data.healthPercent;
                    break;
                case ManaPotionItemData data:
                    value = new ItemManaPotion();
                    ((ItemManaPotion) value).manaPercent = data.manaPercent;
                    break;
                // Weapons
                case SwordWeaponItemData data:
                    value = new ItemSwordWeapon();
                    ((ItemSwordWeapon) value).damage = data.damage;
                    break;
                case ShieldWeaponItemData data:
                    value = new ItemShieldWeapon();
                    ((ItemShieldWeapon) value).damageAbsorption = data.damageAbsorption;
                    break;
                case BowWeaponItemData data:
                    value = new ItemBowWeapon();
                    ((ItemBowWeapon) value).damage = data.damage;
                    ((ItemBowWeapon) value).distance = data.distance;
                    break;
                // Tools
                case ToolItemData data:
                    value = new ItemTool();
                    ((ItemTool)value).durability = data.durability;
                    break;
            }
            return value;
        }
    }
}