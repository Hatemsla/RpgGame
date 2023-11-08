using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.Configurations;
using World.Player;

namespace World.Ability
{
    public class SpellInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        
        private const int SpellPreloadCount = 20;

        public void Init(IEcsSystems systems)
        {
            _ps.Value.SpellPool = new PoolBase<SpellObject>(Preload, GetAction, ReturnAction, SpellPreloadCount);
        }

        private SpellObject Preload() => Object.Instantiate(_cf.Value.abilityConfiguration.abilityDatas[0].spell, 
            Vector3.zero, Quaternion.identity);

        private void GetAction(SpellObject spellObject) => spellObject.gameObject.SetActive(true);
        private void ReturnAction(SpellObject spellObject) => spellObject.gameObject.SetActive(false);
    }
}