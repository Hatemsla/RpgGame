using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.Configurations;
using World.Network;
using World.Player;

namespace World.Ability
{
    public class SpellInitSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        private readonly EcsCustomInject<NetworkRunnerService> _nrs = default;
        
        private const int SpellPreloadCount = 20;
        
        public void Init(IEcsSystems systems)
        {
            if(!_nrs.Value.isPlayerJoined)
                return;

            for (var i = 0; i < _cf.Value.abilityConfiguration.abilityDatas.Count; i++)
            {
                _ps.Value.SpellPool = new PoolBase<SpellObject>(() => Preload(i), GetAction, ReturnAction, SpellPreloadCount);   
            }
        }

        private SpellObject Preload(int index) => Object.Instantiate(_cf.Value.abilityConfiguration.abilityDatas[index].spell, 
            Vector3.zero, Quaternion.identity);

        private void GetAction(SpellObject spellObject) => spellObject.gameObject.SetActive(true);
        private void ReturnAction(SpellObject spellObject) => spellObject.gameObject.SetActive(false);
    }
}