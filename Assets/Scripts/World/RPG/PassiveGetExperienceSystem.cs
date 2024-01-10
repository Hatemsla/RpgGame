using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Player;

namespace World.RPG
{
    public class PassiveGetExperienceSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp, LevelComp>> _filter = default;
        private readonly EcsPoolInject<LevelChangedEvent> _levelChangedEvent = Idents.Worlds.Events;

        private readonly EcsWorldInject _world = Idents.Worlds.Events;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private float _lastExperience = 5f;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var rpgComp = ref _filter.Pools.Inc2.Get(entity);
                
                if (!rpgComp.IsDead && _lastExperience < _ts.Value.Time)
                {
                    ref var levelComp = ref _filter.Pools.Inc3.Get(entity);
                    var levelEntity = _world.Value.NewEntity();
                    ref var levelEventComp = ref _levelChangedEvent.Value.Add(levelEntity);
                    levelEventComp.NewExperience =
                        levelComp.Experience + _cf.Value.playerConfiguration.experiencePassiveIncrease;

                    _lastExperience += _cf.Value.playerConfiguration.experienceIncreaseDelay;
                }
            }
        }
    }
}