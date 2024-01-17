using Leopotam.EcsLite;
using UnityEngine;
using World.Ability.AbilitiesTypes;
using World.AI;
using World.Player;
using World.RPG;

namespace World.Ability.AbilitiesObjects
{
    public class BallAbilityObject : DirectionalAbilityObject
    {
        public TimeService TimeService;
        [HideInInspector] public float speed;
        [HideInInspector] public float startTime;
        
        private void Update()
        {
            var distanceCovered = (TimeService.Time - startTime) * speed;
            var journeyFraction = distanceCovered / direction;
            transform.position = Vector3.Lerp(startDirection, endDirection, journeyFraction);

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
                    
                    enemyRpgComp.Health -= damage;

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
            if (AbilityIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var spell = ref _releasedAbilityPool.Get(unpackedEntity);
                
                PoolService.SpellPool.Return(spell.abilityObject);
                _releasedAbilityPool.Del(unpackedEntity);
            }
        }

        /*public override void Cast()
        {
        }*/
    }
}