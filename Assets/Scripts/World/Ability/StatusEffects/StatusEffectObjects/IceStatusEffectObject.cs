using System;
using Leopotam.EcsLite;
using UnityEngine;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.AI;
using World.AI.Navigation;

namespace World.Ability.StatusEffects.StatusEffectObjects
{
    public class IceStatusEffectObject : StatusEffectObject
    {
        public float lifeTime;
        public float slowDown;
        public EnemyView enemyObj;

        private float _enemyNormalizeSpeed;

        private void Start()
        {
            if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
            {
                var enemyPool = World.GetPool<EnemyComp>();
                ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                
                _enemyNormalizeSpeed = enemyComp.RunSpeed;
            }
            
            InvokeRepeating(nameof(EnemySlovingDown), 0, 1);
        }

        private void Update()
        {
            if (lifeTime > 0)
            {
                lifeTime -= Ts.DeltaTime;

                if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var enemyPool = World.GetPool<EnemyComp>();

                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);

                    transform.position = enemyComp.Transform.position;

                }
            }
            else
            {
                DestroyEffect();
            }
        }

        private void DestroyEffect()
        {
            if (EffectIdx.Unpack(World, out var unpackedEntity))
            {
                var releasedEffectPool = World.GetPool<ReleasedStatusEffectComp>();

                ref var releasedEffectComp = ref releasedEffectPool.Get(unpackedEntity);
                    
                Ps.FireStatusEffectPool.Return(releasedEffectComp.statusEffectObject);
                releasedEffectPool.Del(unpackedEntity);
            }
        }
        
        private void EnemySlovingDown()
        {
            if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
            {
                var enemyPool = World.GetPool<EnemyComp>();
                ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                
                if (lifeTime > 0)
                {

                    enemyComp.RunSpeed -= slowDown;
                    enemyComp.WalkSpeed -= slowDown;
                }
                else
                {
                    if (_enemyNormalizeSpeed != enemyComp.RunSpeed)
                    {
                        enemyComp.RunSpeed += slowDown;
                        enemyComp.WalkSpeed += slowDown;
                    }
                    else
                    {
                        CancelInvoke(nameof(EnemySlovingDown));
                    }
                }
            }
        }

        public override void Applying(EnemyView enemyView, StatusEffectComp effect)
        {
            lifeTime = effect.statusEffectLifeTime;
            slowDown = ((IceStatusEffect)effect.statusEffectType).SlowDown;
            enemyObj = enemyView;
        }
    }
}