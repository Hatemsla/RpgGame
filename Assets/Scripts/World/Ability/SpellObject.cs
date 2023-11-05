using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Inventory;
using World.Player;

namespace World.Ability
{
    public class SpellObject : MonoBehaviour
    {
        public float spellSpeed;
        public float spellDirection;
        public float spellTime;
        public Vector3 spellStart;
        public Vector3 spellEnd;

        public EcsPackedEntity spellIdx;
        public PoolService _ps;

        private EcsWorld _world;
        private int _playerEntity;
        private EcsPool<HasAbilities> _hasAbilities;
        private EcsPool<SpellComp> _spellPool;
        private EcsPool<AbilityComp> _abilityPool;

        private void Update()
        {
            float distanceCovered = (Time.time - spellTime) * spellSpeed;
            float journeyFraction = distanceCovered / spellDirection;
            transform.position = Vector3.Lerp(spellStart, spellEnd, journeyFraction);

            if (journeyFraction >= 1.0f)
            {
                DestroySpell();
            }
        }

        private void DestroySpell()
        {
            if (spellIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var spell = ref _spellPool.Get(unpackedEntity);
                
                _ps.spellPool.Return(spell.spellObject);
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