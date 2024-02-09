﻿using System;
using System.Globalization;
using Leopotam.EcsLite;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.AI;
using World.AI.Navigation;
using World.RPG;
using World.UI.PopupText;

namespace World.Ability.StatusEffects.StatusEffectObjects
{
    public class EarthStatusEffectObject : StatusEffectObject
    {
        public float lifeTime;
        public float damage;
        public EnemyView enemyObj;

        private float _normalizeRunSpeed, _normalizeWalkSpeed;
        private void Start()
        {
            if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
            {
                var enemyPool = World.GetPool<EnemyComp>();
                ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                
                _normalizeRunSpeed = enemyComp.RunSpeed;
                _normalizeWalkSpeed = enemyComp.WalkSpeed;
                enemyComp.RunSpeed = 2;
                enemyComp.WalkSpeed = 2;
            }
        }

        private void Update()
        {
            if (enemyObj.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
            {
                var enemyPool = World.GetPool<EnemyComp>();
                var enemyRpgPool = World.GetPool<RpgComp>();
                var hasEnemiesPool = World.GetPool<HasEnemies>();
                var popupDamageTextPool = World.GetPool<PopupDamageTextComp>();

                ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                
                if (lifeTime > 0)
                {
                    lifeTime -= Ts.DeltaTime;
                    transform.position = enemyComp.Transform.position;
                }
                else
                {
                    enemyComp.RunSpeed = _normalizeRunSpeed;
                    enemyComp.WalkSpeed = _normalizeWalkSpeed;
                    enemyRpgComp.Health -= damage;
                    ShowPopupDamage(popupDamageTextPool, damage, enemyComp);

                    if (enemyRpgComp.Health <= 0)
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
                    }
                    DestroyEffect();
                }
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
        
        private void ShowPopupDamage(EcsPool<PopupDamageTextComp> popupDamageTextPool, float targetDamage, EnemyComp enemyComp)
        {
            ref var popupDamageTextComp = ref popupDamageTextPool.Add(World.NewEntity());
            popupDamageTextComp.LifeTime = Cf.uiConfiguration.popupDamageLifeTime;
            popupDamageTextComp.Damage = targetDamage;
            popupDamageTextComp.Position = enemyComp.EnemyView.transform.position;
            var popupDamageText = Ps.PopupDamageTextPool.Get();
            popupDamageText.damageText.text = popupDamageTextComp.Damage.ToString(CultureInfo.InvariantCulture);
            popupDamageText.transform.position = popupDamageTextComp.Position;
            popupDamageText.currentTime = 0;
            popupDamageTextComp.PopupDamageText = popupDamageText;
            popupDamageTextComp.IsVisible = true;
        }

        public override void Applying(EnemyView enemyView, StatusEffectComp effect)
        {
            lifeTime = effect.statusEffectLifeTime;
            damage = ((EarthStatusEffect)effect.statusEffectType).Damage;
            enemyObj = enemyView;
        }
    }
}