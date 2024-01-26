using System;
using Leopotam.EcsLite;
using UnityEngine;
using World.AI;
using World.AI.Navigation;
using World.RPG;

namespace World.Inventory.WeaponObject
{
    public sealed class Sword : WeaponObject
    {
        private bool _isAttacking;

        private void Update()
        {
            _isAttacking = AnimationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack_OneHanded");
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
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    
                    enemyRpgComp.Health -= damage;

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
                
                // _isAttacking = false;
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