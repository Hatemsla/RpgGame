using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Leopotam.EcsLite;
using ObjectsPool;
using UnityEngine;
using World.Configurations;
using World.Inventory;
using World.Inventory.ItemTypes;
using World.Inventory.ItemTypes.Potions;
using World.Trader.UI;
using World.UI.LookOnObject;

namespace World.Trader
{
    public sealed class Trader : LookOnObject
    {
        public int goldAmount;
        public float sellPercent = 0.5f;
        public List<ItemData> buyItemsDatas;

        public List<BuyPanelView> buyPanelViews;
        public List<SellPanelView> sellPanelViews;
        
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
        
        public override void StartInteract()
        {
            if (_sd.uiSceneData.traderShopView.gameObject.activeInHierarchy)
            {
                _sd.uiSceneData.traderShopView.gameObject.SetActive(false);

                foreach (var t in buyPanelViews) Destroy(t.gameObject);
                foreach (var t in sellPanelViews) Destroy(t.gameObject);

                buyPanelViews.Clear();
                sellPanelViews.Clear();
            }
            else
            {
                _sd.uiSceneData.traderShopView.gameObject.SetActive(true);
                _sd.uiSceneData.playerInventoryView.gameObject.SetActive(false);
                _sd.uiSceneData.traderShopView.traderShopPageForBuy.gameObject.SetActive(true);
                _sd.uiSceneData.traderShopView.traderShopPageForSell.gameObject.SetActive(false);

                _sd.uiSceneData.traderShopView.currentTrader = this;
                
                _sd.uiSceneData.traderShopView.traderCoins.text = "Монеты торговца: " + goldAmount;

                foreach (var buyItemsData in buyItemsDatas)
                {
                    var buyPanelView = Instantiate(_cf.uiConfiguration.buyPanelViewPrefab, _sd.uiSceneData.traderShopView.traderShopPageForBuy.content);
                    buyPanelView.itemData = buyItemsData;
                    buyPanelView.itemName.text = buyItemsData.itemName;
                    buyPanelView.itemImage.sprite = buyItemsData.itemSprite;
                    buyPanelView.price.text = buyItemsData.cost.ToString(CultureInfo.InvariantCulture);
                    buyPanelView.itemCost = buyItemsData.cost;
                    buyPanelView.newItemCost = buyItemsData.cost;
                    buyPanelView.buyCount.text = buyPanelView.buySlider.maxValue.ToString(CultureInfo.InvariantCulture);
                    buyPanelView.SetWorld(_world, _eventWorld, _traderEntity, _playerEntity, _sd, _cf, _ps, _ts);
                    buyPanelViews.Add(buyPanelView);
                }

                var itemPool = _world.GetPool<ItemComp>();
                var hasItemsPool = _world.GetPool<HasItems>();
                
                ref var hasItemsComp = ref hasItemsPool.Get(_playerEntity);

                foreach (var itemPacked in hasItemsComp.Entities)
                {
                    if (itemPacked.Unpack(_world, out var unpackedEntity))
                    {
                        ref var itemComp = ref itemPool.Get(unpackedEntity);
                        switch (itemComp.ItemType)
                        {
                            case ItemPotion:
                                var sellPanelView = Instantiate(_cf.uiConfiguration.sellPanelViewPrefab, _sd.uiSceneData.traderShopView.traderShopPageForSell.content);
                                sellPanelView.itemName.text = itemComp.ItemName;
                                sellPanelView.itemImage.sprite = itemComp.ItemView.itemImage.sprite;
                                sellPanelView.price.text = ((int)(itemComp.Cost * sellPercent)).ToString(CultureInfo.InvariantCulture);
                                sellPanelView.itemCost = (int)(itemComp.Cost * sellPercent);
                                sellPanelView.SetWorld(_world, _eventWorld, _traderEntity, _playerEntity, unpackedEntity, _sd, _cf, _ps, _ts);
                                sellPanelViews.Add(sellPanelView);
                                break;
                        }
                    }
                }
            }
        }

        public override void StopInteract()
        {
            _sd.uiSceneData.traderShopView.gameObject.SetActive(false);

            foreach (var t in buyPanelViews) Destroy(t.gameObject);
            foreach (var t in sellPanelViews) Destroy(t.gameObject);
                
            buyPanelViews.Clear();
            sellPanelViews.Clear();
        }
    }
}