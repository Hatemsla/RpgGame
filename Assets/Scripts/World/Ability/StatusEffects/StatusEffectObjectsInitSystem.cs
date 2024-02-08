using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using UnityEngine;
using World.Ability.StatusEffects.StatusEffectObjects;
using World.Configurations;
using Object = UnityEngine.Object;

namespace World.Ability.StatusEffects
{
    public class StatusEffectObjectsInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;

        private const int StatusEffectsPreloadCount = 1;
        
        public void Init(IEcsSystems systems)
        {
            for (var index = 0; index < _cf.Value.statusEffectConfiguration.statusEffectDatas.Count; index++)
            {
                _ps.Value.FireStatusEffectPool = new PoolBase<StatusEffectObject>(Preload(index), GetAction, ReturnAction,
                    StatusEffectsPreloadCount);
                _ps.Value.IceStatusEffectPool = new PoolBase<StatusEffectObject>(Preload(index), GetAction, ReturnAction,
                    StatusEffectsPreloadCount);
            }
        }

        private Func<StatusEffectObject> Preload(int index) => () => Object.Instantiate(_cf.Value
            .statusEffectConfiguration
            .statusEffectDatas[index].statusEffectObjectPrefab, Vector3.zero, Quaternion.identity);

        private void GetAction(StatusEffectObject statusEffectObject) => statusEffectObject.gameObject.SetActive(true);

        private void ReturnAction(StatusEffectObject statusEffectObject) =>
            statusEffectObject.gameObject.SetActive(false);
    }
}