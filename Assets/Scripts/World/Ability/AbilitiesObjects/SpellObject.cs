using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Player;
using World.RPG;

namespace World.Ability.AbilitiesObjects
{
    public class SpellObject : MonoBehaviour
    {
        public float spellDamage;
        public float spellSpeed;
        public float spellDirection;
        public float spellTime;
        public Vector3 spellStart;
        public Vector3 spellEnd;

        public EcsPackedEntity SpellIdx;
        public PoolService PoolService;
        public TimeService TimeService;

        private EcsWorld _world;
        private int _playerEntity;
        private EcsPool<HasAbilities> _hasAbilities;
        private EcsPool<SpellComp> _spellPool;
        private EcsPool<AbilityComp> _abilityPool;

        private void Update()
        {
            var distanceCovered = (TimeService.Time - spellTime) * spellSpeed;
            var journeyFraction = distanceCovered / spellDirection;
            transform.position = Vector3.Lerp(spellStart, spellEnd, journeyFraction);

            if (journeyFraction >= 1.0f)
            {
                DestroySpell();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            var enemyView = other.gameObject.GetComponent<EnemyView>();

            if (enemyView)
            {
                if (enemyView.EnemyPacked.Unpack(_world, out var unpackedEnemyEntity))
                {
                    var enemyPool = _world.GetPool<EnemyComp>();
                    var enemyRpgPool = _world.GetPool<RpgComp>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    
                    enemyRpgComp.Health -= spellDamage;

                    if (enemyRpgComp.Health <= 0)
                    {
                        PoolService.EnemyPool.Return(enemyComp.EnemyView);
                        
                        enemyPool.Del(unpackedEnemyEntity);
                        enemyRpgPool.Del(unpackedEnemyEntity);
                    }
                    
                    DestroySpell();
                }
            }
        }

        private void DestroySpell()
        {
            if (SpellIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var spell = ref _spellPool.Get(unpackedEntity);
                
                PoolService.SpellPool.Return(spell.spellObject);
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