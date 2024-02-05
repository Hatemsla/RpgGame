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
        [HideInInspector] private float lifeTime;
        [HideInInspector] private float damage;
        [HideInInspector] private RpgComp enemyRpg;
        
        private void Update()
        {
            if (lifeTime > 0)
            {
                lifeTime -= Ts.DeltaTime;
                enemyRpg.Health -= damage / lifeTime;
            }
            else
            {
                if (EffectIdx.Unpack(World, out var unpackedEntity))
                {
                    var releasedEffectPool = World.GetPool<ReleasedStatusEffectComp>();

                    ref var releasedEffectComp = ref releasedEffectPool.Get(unpackedEntity);
                    
                    Ps.StatusEffectPool.Return(releasedEffectComp.statusEffectObject);
                    releasedEffectPool.Del(unpackedEntity);
                }
            }
        }

        public override void Applying(EnemyView enemyView, StatusEffectComp effect)
        {
            if (enemyView)
                if (enemyView.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var enemyPool = World.GetPool<EnemyComp>();
                    var enemyRpgPool = World.GetPool<RpgComp>();

                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);

                    lifeTime = effect.statusEffectLifeTime;
                    damage = ((FireStatusEffect)effect.statusEffectType).Damage;
                    enemyRpg = enemyRpgComp;
                }
        }
    }
}