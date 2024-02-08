using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.ObjectsPool;
using World.Configurations;
using World.Inventory;
using World.Player;

namespace World.Trader.UI
{
    public class SellPanelView : MonoBehaviour
    {
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text price;

        public int itemCost;
        private int _lastAmount;
        
        private EcsWorld _world;
        private EcsWorld _eventWorld;
        private int _traderEntity;
        private int _playerEntity;
        private int _itemEntity;
        private SceneData _sd;
        private Configuration _cf;
        private PoolService _ps;
        private TimeService _ts;

        public void SetWorld(EcsWorld world, EcsWorld eventWorld, int traderEntity, int playerEntity, int itemEntity, SceneData sd, Configuration cf, PoolService ps, TimeService ts)
        {
            _world = world;
            _eventWorld = eventWorld;
            _traderEntity = traderEntity;
            _playerEntity = playerEntity;
            _itemEntity = itemEntity;
            _sd = sd;
            _cf = cf;
            _ps = ps;
            _ts = ts;
        }
        
        public void SellItem()
        {
            var itemsPool = _world.GetPool<ItemComp>();
            var playerPool = _world.GetPool<PlayerComp>();
            var inventoryPool = _world.GetPool<InventoryComp>();
            var traderPool = _world.GetPool<TraderComp>();

            ref var playerComp = ref playerPool.Get(_playerEntity);
            ref var itemComp = ref itemsPool.Get(_itemEntity);
            ref var traderComp = ref traderPool.Get(_traderEntity);
            ref var inventoryComp = ref inventoryPool.Get(_playerEntity);
            
            if (traderComp.Trader.goldAmount < itemCost) return;

            playerComp.GoldAmount += itemCost;
            traderComp.Trader.goldAmount -= itemCost;
            
            var coinsAmount = playerComp.GoldAmount.ToString();
            _sd.uiSceneData.playerInventoryGoldAmount.goldAmount.text = coinsAmount;
            _sd.uiSceneData.traderShopView.playerCoins.text = "Мои монеты: " + coinsAmount;
            _sd.uiSceneData.traderShopView.traderCoins.text = "Монеты торговца: " + traderComp.Trader.goldAmount;
            
            foreach (var ft in _sd.fastItemViews)
            {
                if (ft.ItemIdx.Unpack(_world, out var ftUnpackedEntity))
                {
                    if (ftUnpackedEntity == _itemEntity)
                    {
                        Utils.Utils.ResetFastItemView(ft);
                        break;
                    }
                }
            }

            inventoryComp.CurrentWeight -= itemComp.Weight;
            inventoryComp.InventoryWeightView.inventoryWeightText.text =
                $"Вес: {inventoryComp.CurrentWeight:f1}/{inventoryComp.MaxWeight}";
            
            Destroy(itemComp.ItemView.gameObject);
            transform.gameObject.SetActive(false);
            
            itemsPool.Del(_itemEntity);
        }
    }
}