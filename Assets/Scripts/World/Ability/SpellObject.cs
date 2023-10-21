using System;
using Leopotam.EcsLite;
using UnityEngine;
using World.Inventory;
using World.Player;

namespace World.Ability
{
    public class SpellObject : MonoBehaviour
    {
        public EcsPackedEntity spellIdx;
        public float spellTime;
        public float spellSpeed;
        public Vector3 spellDirection;
        
        private EcsWorld _world;
        private int _playerEntity;
        private EcsPool<HasAbilities> _hasAbilities;
        private EcsPool<SpellComp> _spellPool;
        private EcsPool<AbilityComp> _abilityPool; 
        
        private void Update()
        {
            spellTime -= Time.deltaTime;
            if (spellTime > 0)
            {
                transform.Translate(spellDirection  * spellSpeed * Time.deltaTime);
            }
            else
            {
                DestroySpell();
            }
        }

        private void DestroySpell()
        {
            if (spellIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var spell = ref _spellPool.Get(unpackedEntity);
                    
                Destroy(spell.spellObject.gameObject);
                _spellPool.Del(unpackedEntity);
            }
        }
        
        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _playerEntity = entity;
            _spellPool = _world.GetPool<SpellComp>();
            _abilityPool = _world.GetPool<AbilityComp>();
        }
    }
}