using System;
using System.Collections.Generic;
using System.Globalization;
using Leopotam.EcsLite;
using UnityEngine;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;
using World.UI.PopupText;
using Random = UnityEngine.Random;

namespace World.Inventory.WeaponObject
{
    public sealed class Sword : WeaponObject
    {
        private bool _isAttacking;

        private readonly List<EnemyView> _attackedEnemies = new();
        
        private void Update()
        {
            ref var animationComp = ref DefaultWorld.GetPool<AnimationComp>().Get(playerEntity);
            _isAttacking = animationComp.Animator.GetCurrentAnimatorStateInfo(0).IsName("MeleeAttack_OneHanded");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isAttacking)
            {
                if(_attackedEnemies.Count != 0)
                    _attackedEnemies.Clear();
                return;
            }
            
            var enemyView = other.gameObject.GetComponent<EnemyView>();
            
            if (enemyView)
            {
                if(_attackedEnemies.Contains(enemyView))
                    return;
                
                _attackedEnemies.Add(enemyView);

                if (enemyView.EnemyPackedIdx.Unpack(DefaultWorld, out var unpackedEnemyEntity))
                {
                    var enemyPool = DefaultWorld.GetPool<EnemyComp>();
                    var enemyRpgPool = DefaultWorld.GetPool<RpgComp>();
                    var hasEnemiesPool = DefaultWorld.GetPool<HasEnemies>();
                    var levelPool = DefaultWorld.GetPool<LevelComp>();
                    var popupDamageTextPool = DefaultWorld.GetPool<PopupDamageTextComp>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    ref var levelComp = ref levelPool.Get(playerEntity);

                    var targetDamage = DamageEnemy(levelComp, ref enemyRpgComp);

                    ShowPopupDamage(popupDamageTextPool, targetDamage, enemyComp);

                    if (enemyRpgComp.Health <= 0)
                    {
                        Ps.EnemyPool.Return(enemyComp.EnemyView);
                        
                        if (enemyView.ZonePackedIdx.Unpack(DefaultWorld, out var unpackedZoneEntity))
                        {
                            ref var hasEnemyComp = ref hasEnemiesPool.Get(unpackedZoneEntity);

                            for (var index = 0; index < hasEnemyComp.Entities.Count; index++)
                            {
                                var hasEnemyEntityPacked = hasEnemyComp.Entities[index];
                                if (hasEnemyEntityPacked.Unpack(DefaultWorld, out var unpackedHasEnemyEntity))
                                {
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                    {
                                        hasEnemyComp.Entities.RemoveAll(entityPacked => entityPacked.Unpack(DefaultWorld, out var entity) && entity == unpackedEnemyEntity);
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

        private float DamageEnemy(LevelComp levelComp, ref RpgComp enemyRpgComp)
        {
            var crit = Random.Range(-10, 1) + levelComp.Luck;

            var defaultDamageCrit = crit switch
            {
                > 0 => 2,
                < 0 => 1,
                _ => 4
            };

            var targetDamage = damage * (levelComp.PAtk / 100 + 1) * defaultDamageCrit;

            enemyRpgComp.Health -= targetDamage;
            return targetDamage;
        }

        private void ShowPopupDamage(EcsPool<PopupDamageTextComp> popupDamageTextPool, float targetDamage, EnemyComp enemyComp)
        {
            ref var popupDamageTextComp = ref popupDamageTextPool.Add(DefaultWorld.NewEntity());
            popupDamageTextComp.LifeTime = cf.uiConfiguration.popupDamageLifeTime;
            popupDamageTextComp.Damage = targetDamage;
            popupDamageTextComp.Position = enemyComp.EnemyView.transform.position;
            var popupDamageText = Ps.PopupDamageTextPool.Get();
            popupDamageText.damageText.text = popupDamageTextComp.Damage.ToString(CultureInfo.InvariantCulture);
            popupDamageText.transform.position = popupDamageTextComp.Position;
            popupDamageText.currentTime = 0;
            popupDamageTextComp.PopupDamageText = popupDamageText;
            popupDamageTextComp.IsVisible = true;
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