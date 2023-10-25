using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace World.AI.Navigation
{
    public sealed class ZoneInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<ZoneComp> _zonePool = default;

        private readonly EcsWorldInject _world = default; 
        
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var zone in _sd.Value.zones)
            {
                var zoneEntity = _world.Value.NewEntity();

                ref var zoneComp = ref _zonePool.Value.Add(zoneEntity);

                zoneComp.ZoneView = zone;
            }
        }
    }
}