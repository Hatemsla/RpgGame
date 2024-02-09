using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using Utils;
using World.Configurations;
using World.LoadGame;
using World.Player;

namespace World.Trader
{
    public class TraderInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<LoadDataEventComp>> _loadDataFilter = Idents.Worlds.Events;
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;
        private readonly EcsPoolInject<TraderComp> _tradersPool = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<PoolService> _ps = default;

        private readonly EcsWorldInject _defaultWorld = default;
        private readonly EcsWorldInject _eventWorld = Idents.Worlds.Events;

        public void Init(IEcsSystems systems)
        {
            foreach (var loadDataEventEntity in _loadDataFilter.Value)
            {
                ref var loadDataEventComp = ref _loadDataFilter.Pools.Inc1.Get(loadDataEventEntity);
                
                foreach (var playerEntity in _playerFilter.Value)
                {
                    var traderIndex = 0;
                    foreach (var trader in _sd.Value.traders)
                    {
                        if (loadDataEventComp.IsLoadData)
                        {
                            var traderSaveData = loadDataEventComp.TraderSaveDatas.Traders[traderIndex];
                            trader.goldAmount = traderSaveData.GoldAmount;
                            traderIndex++;
                        }

                        var traderEntity = _defaultWorld.Value.NewEntity();

                        ref var traderComp = ref _tradersPool.Value.Add(traderEntity);

                        traderComp.Trader = trader;
                        traderComp.Trader.SetWorld(_defaultWorld.Value, _eventWorld.Value, traderEntity,
                            playerEntity,
                            _sd.Value, _cf.Value, _ps.Value, _ts.Value);
                    }
                }
            }
        }
    }
}