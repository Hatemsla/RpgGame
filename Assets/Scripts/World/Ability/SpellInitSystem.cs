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
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _player = default;
        
        private const int SPELL_PRELOAD_COUNT = 20;

        public void Init(IEcsSystems systems)
        {
            _ps.Value.spellPool = new PoolBase<SpellObject>(Preload, GetAction, ReturnAction, SPELL_PRELOAD_COUNT);
        }
        
        public SpellObject Preload() => Object.Instantiate(_cf.Value.abilityConfiguration.abilityDatas[0].spell, 
            Vector3.zero, Quaternion.identity);
        public void GetAction(SpellObject spellObject) => spellObject.gameObject.SetActive(true);
        public void ReturnAction(SpellObject spellObject) => spellObject.gameObject.SetActive(false);
    }
}