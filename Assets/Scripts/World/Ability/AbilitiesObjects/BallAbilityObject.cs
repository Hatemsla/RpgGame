using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Ability.AbilitiesTypes;
using World.AI;
using World.AI.Navigation;
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
                if (enemyView.EnemyPackedIdx.Unpack(_world, out var unpackedEnemyEntity))
                {
                    var enemyPool = _world.GetPool<EnemyComp>();
                    var enemyRpgPool = _world.GetPool<RpgComp>();
                    var hasEnemiesPool = _world.GetPool<HasEnemies>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    
                    enemyRpgComp.Health -= damage;

                    if (enemyRpgComp.Health <= 0)
                    {
                        PoolService.EnemyPool.Return(enemyComp.EnemyView);

                        if (enemyView.ZonePackedIdx.Unpack(_world, out var unpackedZoneEntity))
                        {
                            ref var hasEnemyComp = ref hasEnemiesPool.Get(unpackedZoneEntity);
                            Debug.Log(hasEnemyComp.Entities.Count);

                            for (var index = 0; index < hasEnemyComp.Entities.Count; index++)
                            {
                                var hasEnemyEntityPacked = hasEnemyComp.Entities[index];
                                if (hasEnemyEntityPacked.Unpack(_world, out var unpackedHasEnemyEntity))
                                {
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                    {
                                        // hasEnemyComp.Entities[index] = default;
                                        // hasEnemyComp.Entities.Remove(hasEnemyEntityPacked);
                                        hasEnemyComp.Entities.RemoveAll(entityPacked => entityPacked.Unpack(_world, out var entity) && entity == unpackedEnemyEntity);
                                        Debug.Log("Removed");
                                    }
                                }
                            }

                            enemyPool.Del(unpackedEnemyEntity);
                            enemyRpgPool.Del(unpackedEnemyEntity);
                        }
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