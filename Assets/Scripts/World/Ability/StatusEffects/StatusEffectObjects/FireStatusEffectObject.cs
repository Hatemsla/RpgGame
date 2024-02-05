using System;
using Leopotam.EcsLite;
using UnityEngine;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;

namespace World.Ability.StatusEffects.StatusEffectObjects
{
    public class FireStatusEffectObject : StatusEffectObject
    {
        public float lifeTime;
        public float damage;
        public EnemyView enemyObj;
        
        private void Update()
        {
            if (lifeTime > 0)
            {
                lifeTime -= Ts.DeltaTime;

                if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var enemyPool = World.GetPool<EnemyComp>();
                    var enemyRpgPool = World.GetPool<RpgComp>();
                    var hasEnemiesPool = World.GetPool<HasEnemies>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);

                    transform.position = enemyComp.Transform.position;
                    if (enemyRpgComp.Health > 0)
                    {
                        enemyRpgComp.Health -= damage / lifeTime;
                    }
                    else
                    {
                        Ps.EnemyPool.Return(enemyComp.EnemyView);

                        if (enemyObj.ZonePackedIdx.Unpack(World, out var unpackedZoneEntity))
                        {
                            ref var hasEnemyComp = ref hasEnemiesPool.Get(unpackedZoneEntity);
                            
                            for (var index = 0; index < hasEnemyComp.Entities.Count; index++)
                            {
                                var hasEnemyEntityPacked = hasEnemyComp.Entities[index];
                                if (hasEnemyEntityPacked.Unpack(World, out var unpackedHasEnemyEntity))
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                        hasEnemyComp.Entities.RemoveAll(entityPacked =>
                                            entityPacked.Unpack(World, out var entity) &&
                                            entity == unpackedEnemyEntity);
                            }

                            enemyPool.Del(unpackedEnemyEntity);
                            enemyRpgPool.Del(unpackedEnemyEntity);
                        }
                        
                        DestroyEffect();
                    }
                }
            }
            else
                DestroyEffect();
        }

        private void DestroyEffect()
        {
            if (EffectIdx.Unpack(World, out var unpackedEntity))
            {
                var releasedEffectPool = World.GetPool<ReleasedStatusEffectComp>();

                ref var releasedEffectComp = ref releasedEffectPool.Get(unpackedEntity);
                    
                Ps.StatusEffectPool.Return(releasedEffectComp.statusEffectObject);
                releasedEffectPool.Del(unpackedEntity);
            }
        }

        public override void Applying(EnemyView enemyView, StatusEffectComp effect)
        {
            //lifeTime = effect.statusEffectLifeTime;
            lifeTime = 20;
            //damage = ((FireStatusEffect)effect.statusEffectType).Damage;
            damage = 15;
            enemyObj = enemyView;
            
            /*if (enemyView.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
            {
                var enemyPool = World.GetPool<EnemyComp>();
                var enemyRpgPool = World.GetPool<RpgComp>();

                ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);

                transform.position = enemyComp.Transform.position;

                //lifeTime = effect.statusEffectLifeTime;
                lifeTime = 20;
                //damage = ((FireStatusEffect)effect.statusEffectType).Damage;
                damage = 15;
                enemyObj = enemyView;
            }*/
        }
    }
}