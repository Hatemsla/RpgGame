using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Inventory.ItemTypes.Weapons;
using World.Inventory.WeaponObject;
using World.Player;
using World.Player.Weapons.WeaponViews;
using World.RPG;

namespace World.Inventory
{
    public class ItemsInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, InventoryComp, AnimationComp, LevelComp>> _playerFilter =
            default;

        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        private readonly EcsWorldInject _world = default;
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;

        public void Init(IEcsSystems systems)
        {
            _sd.Value.uiSceneData.deleteFormView.gameObject.SetActive(false);

            foreach (var playerEntity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);
                ref var inventoryComp = ref _playerFilter.Pools.Inc2.Get(playerEntity);

                inventoryComp.MaxWeight = _cf.Value.inventoryConfiguration.inventoryWeight;
                inventoryComp.CurrentWeight = 0f;

                _sd.Value.uiSceneData.playerInventoryView.gameObject.SetActive(false);

                ref var hasItems = ref _hasItemsPool.Value.Add(playerEntity);
                var items = _cf.Value.inventoryConfiguration.items;
                var playerInventoryViewContent =
                    _sd.Value.uiSceneData.playerInventoryView.GetComponentInChildren<ContentView>();
                playerInventoryViewContent.currentEntity = playerEntity;

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
                    it.ItemType = Utils.Utils.DefineItemType(itemData.itemTypeData);

                    weight += itemData.itemWeight;

                    var itemView = Object.Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                    itemView.transform.SetParent(playerInventoryViewContent.transform);
                    it.ItemView = itemView;

                    it.ItemView.itemImage.sprite = itemData.itemSprite;
                    it.ItemView.ItemIdx = itemPackedEntity;
                    it.ItemView.ItemName = itemData.itemName;
                    it.ItemView.ItemDescription = itemData.itemDescription;
                    it.ItemView.ItemCount = itemData.itemCount.ToString();
                    it.ItemView.SetWorld(_world.Value, _eventWorld.Value, playerEntity, _sd.Value);

                    it.ItemView.SetViews(_sd.Value.uiSceneData.playerInventoryView,
                        _sd.Value.uiSceneData.chestInventoryView, _sd.Value.uiSceneData.fastItemsView,
                        _sd.Value.uiSceneData.deleteFormView, _sd.Value.uiSceneData.crosshairView);

                    if (itemData.itemObjectPrefab)
                    {
                        var rightArm = playerComp.Transform.GetComponentInChildren<RightArmView>().transform;
                        var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                            rightArm.position,
                            Quaternion.identity);

                        itemObject.DefaultWorld = _world.Value;
                        itemObject.EventWorld = _eventWorld.Value;
                        itemObject.playerEntity = playerEntity;
                        itemObject.Ps = _ps.Value;
                        itemObject.Ts = _ts.Value;
                        itemObject.cf = _cf.Value;

                        itemObject.transform.SetParent(rightArm);
                        itemObject.transform.localPosition = itemData.itemObjectPrefab.transform.position;
                        itemObject.transform.localRotation = itemData.itemObjectPrefab.transform.rotation;

                        switch (itemObject)
                        {
                            case Sword sword:
                                sword.damage = ((ItemSwordWeapon)it.ItemType).Damage;
                                sword.wasteStamina = ((ItemSwordWeapon)it.ItemType).WasteStamina;
                                break;
                        }

                        itemObject.gameObject.SetActive(false);

                        it.ItemView.itemObject = itemObject;
                        it.ItemView.itemObject.ItemIdx = itemPackedEntity;
                        _sd.Value.fastItemViews[i].itemObject = itemObject;
                        _sd.Value.fastItemViews[i].itemObject.ItemIdx = itemPackedEntity;
                    }

                    _sd.Value.fastItemViews[i].ItemIdx = itemPackedEntity;
                    _sd.Value.fastItemViews[i].itemImage.sprite = itemData.itemSprite;
                    _sd.Value.fastItemViews[i].itemName.text = itemData.itemName;
                    _sd.Value.fastItemViews[i].itemCount.text = itemData.itemCount.ToString();

                    hasItems.Entities.Add(itemPackedEntity);
                }

                inventoryComp.CurrentWeight = weight;

                inventoryComp.InventoryWeightView = _sd.Value.uiSceneData.playerInventoryWeightText;
                inventoryComp.InventoryWeightView.inventoryWeightText.text =
                    $"Вес: {inventoryComp.CurrentWeight:f1}/{inventoryComp.MaxWeight}";
            }
        }
    }
}