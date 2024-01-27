using System;
using Leopotam.EcsLite;
using UnityEngine;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;

namespace World.Inventory.WeaponObject
{
    public sealed class Sword : WeaponObject
    {
        private bool _isAttacking;

        private void Update()
        {
            ref var animationComp = ref World.GetPool<AnimationComp>().Get(playerEntity);
            _isAttacking = animationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack_OneHanded");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAttacking) return;
            
            var enemyView = other.gameObject.GetComponent<EnemyView>();
            
            if (enemyView)
            {
                if (enemyView.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var enemyPool = World.GetPool<EnemyComp>();
                    var enemyRpgPool = World.GetPool<RpgComp>();
                    var hasEnemiesPool = World.GetPool<HasEnemies>();
                    var levelPool = World.GetPool<LevelComp>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    ref var levelComp = ref levelPool.Get(playerEntity);
                    
                    enemyRpgComp.Health -= damage * (levelComp.PAtk / 100 + 1);

                    if (enemyRpgComp.Health <= 0)
                    {
                        Ps.EnemyPool.Return(enemyComp.EnemyView);
                        
                        if (enemyView.ZonePackedIdx.Unpack(World, out var unpackedZoneEntity))
                        {
                            ref var hasEnemyComp = ref hasEnemiesPool.Get(unpackedZoneEntity);

                            for (var index = 0; index < hasEnemyComp.Entities.Count; index++)
                            {
                                var hasEnemyEntityPacked = hasEnemyComp.Entities[index];
                                if (hasEnemyEntityPacked.Unpack(World, out var unpackedHasEnemyEntity))
                                {
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                    {
                                        hasEnemyComp.Entities.RemoveAll(entityPacked => entityPacked.Unpack(World, out var entity) && entity == unpackedEnemyEntity);
                                    }
                                }
                            }

                            enemyPool.Del(unpackedEnemyEntity);
                            enemyRpgPool.Del(unpackedEnemyEntity);
                        }
                    }
                }
            }
        }

        public override void Attack()
        {
            // _isAttacking = true;
            // Debug.Log(AnimationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack_OneHanded"));
            // if (!AnimationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack_OneHanded"))
            //     _isAttacking = true;
        }
    }
}