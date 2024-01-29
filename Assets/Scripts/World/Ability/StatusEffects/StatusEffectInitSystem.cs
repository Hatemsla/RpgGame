using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.Ability.StatusEffects.AbilityStatusEffectData;
using World.Configurations;
using World.Player;
using Object = UnityEngine.Object;

namespace World.Ability.StatusEffects
{
    public sealed class StatusEffectInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;

        private readonly EcsPoolInject<HasStatusEffect> _hasStatusEffectPool = default;
        private readonly EcsPoolInject<StatusEffectComp> _statusEffectPool = default;

        private readonly EcsCustomInject<Configuration> _cf = default;

        private readonly EcsWorldInject _world = default;

        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);
                ref var hasStatusEffect = ref _hasStatusEffectPool.Value.Add(entity);
                var statusEffects = _cf.Value.statusEffectConfiguration.statusEffectDatas;

                for (var i = 0; i < statusEffects.Count; i++)
                {
                    var statusEffectData = statusEffects[i];
                    var statusEffectEntity = _world.Value.NewEntity();
                    var statusEffectPackedEntity = _world.Value.PackEntity(statusEffectEntity);
                    ref var statusEffectComp = ref _statusEffectPool.Value.Add(statusEffectEntity);

                    statusEffectComp.statusEffectLifeTime = statusEffectData.statusEffectLifeTime;
                    statusEffectComp.statusEffectType = DefineStatusEffectType(statusEffectData.statusEffectTypeData);

                    var statusEffectObject = Object.Instantiate(statusEffectData.statusEffectObject1Prefab,
                        playerComp.Transform.position + playerComp.Transform.forward,
                        statusEffectData.statusEffectObject1Prefab.transform.rotation);
                    statusEffectObject.transform.SetParent(playerComp.Transform);
                    statusEffectObject.gameObject.SetActive(false);
                    
                    hasStatusEffect.Entities.Add(statusEffectPackedEntity);
                }
            }
        }

        private StatusEffectType DefineStatusEffectType(StatusEffectTypeData statusEffectTypeData)
        {
            StatusEffectType value = null;
            switch (statusEffectTypeData)
            {
                case FireStatusEffectData data:
                    value = new FireStatusEffect();
                    ((FireStatusEffect)value).Damage = data.damage;
                    break;
            }

            return value;
        }
    }
}