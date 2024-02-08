using System.Globalization;
using Leopotam.EcsLite;
using ObjectsPool;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World.Configurations;
using World.Inventory;
using World.Inventory.ItemTypes;
using World.Inventory.ItemTypes.Weapons;
using World.Inventory.WeaponObject;
using World.Player;
using World.Player.Weapons.WeaponViews;

namespace World.Trader.UI
{
    public class BuyPanelView : MonoBehaviour
    {
        public ItemData itemData;
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text buyCount;
        public Slider buySlider;
        public TMP_Text price;

        public int itemCost;
        public int newItemCost;
        private int _lastAmount;
        
        private EcsWorld _world;
        private EcsWorld _eventWorld;
        private int _traderEntity;
        private int _playerEntity;
        private SceneData _sd;
        private Configuration _cf;
        private PoolService _ps;
        private TimeService _ts;

        public void SetWorld(EcsWorld world, EcsWorld eventWorld, int traderEntity, int playerEntity, SceneData sd, Configuration cf, PoolService ps, TimeService ts)
        {
            _world = world;
            _eventWorld = eventWorld;
            _traderEntity = traderEntity;
            _playerEntity = playerEntity;
            _sd = sd;
            _cf = cf;
            _ps = ps;
            _ts = ts;
        }
        
        public void BuyItem()
        {
            var itemsPool = _world.GetPool<ItemComp>();
            var playerPool = _world.GetPool<PlayerComp>();
            var inventoryPool = _world.GetPool<InventoryComp>();
            var hasItemsPool = _world.GetPool<HasItems>();
            var traderPool = _world.GetPool<TraderComp>();

            ref var playerComp = ref playerPool.Get(_playerEntity);
            ref var traderComp = ref traderPool.Get(_traderEntity);
            ref var inventoryComp = ref inventoryPool.Get(_playerEntity);
            ref var hasItemsComp = ref hasItemsPool.Get(_playerEntity);
            
            if (playerComp.GoldAmount < newItemCost) return;

            playerComp.GoldAmount -= newItemCost;
            traderComp.Trader.goldAmount += newItemCost;
            
            var coinsAmount = playerComp.GoldAmount.ToString();
            _sd.uiSceneData.playerInventoryGoldAmount.goldAmount.text = coinsAmount;
            _sd.uiSceneData.traderShopView.playerCoins.text = "Мои монеты: " + coinsAmount;
            _sd.uiSceneData.traderShopView.traderCoins.text = "Монеты торговца: " + traderComp.Trader.goldAmount;
            
            var playerInventoryViewContent =
                _sd.uiSceneData.playerInventoryView.GetComponentInChildren<ContentView>().transform;

            var weight = 0f;
            for (int i = 0; i < buySlider.value; i++)
            {
                var itemEntity = _world.NewEntity();
                var itemPackedEntity = _world.PackEntity(itemEntity);
                ref var it = ref itemsPool.Add(itemEntity);
            
                it.ItemName = itemData.itemName;
                it.ItemDescription = itemData.itemDescription;
                it.Cost = itemData.cost;
                it.Weight = itemData.itemWeight;
                it.ItemType = Utils.Utils.DefineItemType(itemData.itemTypeData);
                
                weight += itemData.itemWeight;

                var itemView = Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                itemView.transform.SetParent(playerInventoryViewContent);
                it.ItemView = itemView;
                
                it.ItemView.itemImage.sprite = itemData.itemSprite;
                it.ItemView.ItemIdx = itemPackedEntity;
                it.ItemView.ItemName = itemData.itemName;
                it.ItemView.ItemDescription = itemData.itemDescription;
                it.ItemView.ItemCount = itemData.itemCount.ToString();
                it.ItemView.SetWorld(_world, _eventWorld, _playerEntity, _sd);

                it.ItemView.SetViews(_sd.uiSceneData.playerInventoryView,
                    _sd.uiSceneData.chestInventoryView, _sd.uiSceneData.fastItemsView,
                    _sd.uiSceneData.deleteFormView, _sd.uiSceneData.crosshairView);

                if (itemData.itemObjectPrefab)
                {
                    var rightArm = playerComp.Transform.GetComponentInChildren<RightArmView>().transform;
                    var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                        rightArm.position,
                        Quaternion.identity);

                    itemObject.DefaultWorld = _world;
                    itemObject.EventWorld = _eventWorld;
                    itemObject.playerEntity = _playerEntity;
                    itemObject.Ps = _ps;
                    itemObject.Ts = _ts;
                    itemObject.cf = _cf;

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
                }

                hasItemsComp.Entities.Add(itemPackedEntity);
            }

            inventoryComp.CurrentWeight += weight;

            inventoryComp.InventoryWeightView.inventoryWeightText.text =
                $"Вес: {inventoryComp.CurrentWeight:f1}/{inventoryComp.MaxWeight}";
            
            foreach (var t in traderComp.Trader.sellPanelViews) Destroy(t.gameObject);
            traderComp.Trader.sellPanelViews.Clear();
            
            foreach (var itemPacked in hasItemsComp.Entities)
            {
                if (itemPacked.Unpack(_world, out var unpackedEntity))
                {
                    ref var itemComp = ref itemsPool.Get(unpackedEntity);
                    switch (itemComp.ItemType)
                    {
                        case ItemPotion:
                            var sellPanelView = Instantiate(_cf.uiConfiguration.sellPanelViewPrefab, _sd.uiSceneData.traderShopView.traderShopPageForSell.content);
                            sellPanelView.itemName.text = itemComp.ItemName;
                            sellPanelView.itemImage.sprite = itemComp.ItemView.itemImage.sprite;
                            sellPanelView.price.text = ((int)(itemComp.Cost * traderComp.Trader.sellPercent)).ToString(CultureInfo.InvariantCulture);
                            sellPanelView.itemCost = (int)(itemComp.Cost * traderComp.Trader.sellPercent);
                            sellPanelView.SetWorld(_world, _eventWorld, _traderEntity, _playerEntity, unpackedEntity, _sd, _cf, _ps, _ts);
                            traderComp.Trader.sellPanelViews.Add(sellPanelView);
                            break;
                    }
                }
            }
        }

        public void OnChangeItemAmount(float value)
        {
            buyCount.text = ((int)value).ToString();
            
            if(value > _lastAmount)
                newItemCost += itemCost;
            else
                newItemCost -= itemCost;

            price.text = newItemCost.ToString();

            _lastAmount = (int)value;
        }
    }
}