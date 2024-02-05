using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using UnityEngine;
using Utils.ObjectsPool;
using World.Ability.AbilitiesObjects;
using World.Configurations;
using Object = UnityEngine.Object;

namespace World.Ability
{
    public class AbilityObjectsInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        
        private const int SpellPreloadCount = 20;

        public void Init(IEcsSystems systems)
        {
            for (var index = 0; index < _cf.Value.abilityConfiguration.abilityDatas.Count; index++)
            {
                _ps.Value.SpellPool = new PoolBase<AbilityObject>(Preload(index), GetAction, ReturnAction, 
                    SpellPreloadCount);
            }
        }

        private Func<AbilityObject> Preload(int index) => () => Object.Instantiate(_cf.Value.abilityConfiguration
                .abilityDatas[index].abilityObjectPrefab, Vector3.zero, Quaternion.identity);

        private void GetAction(AbilityObject abilityObject) => abilityObject.gameObject.SetActive(true);
        private void ReturnAction(AbilityObject abilityObject) => abilityObject.gameObject.SetActive(false);
    }
}