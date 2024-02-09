using System;
using System.Globalization;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Rendering;
using World.Ability.AbilitiesTypes;
using World.Ability.StatusEffects;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.Ability.StatusEffects.StatusEffectObjects;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;
using World.UI.PopupText;
using Random = UnityEngine.Random;

namespace World.Ability.AbilitiesObjects
{
    public abstract class BallAbilityObject : DirectionalAbilityObject
    {
        public float speed;
        public float startTime;
        private static readonly int MoveX = Animator.StringToHash("MoveX");

        public void Start()
        {
            Vector3 newRotation = new Vector3(90, 0, 0);
            transform.Rotate(newRotation);
        }

        public virtual void Update()
        {
            var distanceCovered = (Ts.Time - startTime) * speed;
            var journeyFraction = distanceCovered / direction;
            transform.position = Vector3.Lerp(startDirection, endDirection, journeyFraction);

            if (journeyFraction >= 1.0f) DestroySpell();
        }

        public virtual void OnCollisionEnter(Collision other)
        {
            var enemyView = other.gameObject.GetComponent<EnemyView>();

            if (enemyView)
                if (enemyView.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var playerPool = World.GetPool<PlayerComp>();
                    var enemyPool = World.GetPool<EnemyComp>();
                    var enemyRpgPool = World.GetPool<RpgComp>();
                    var hasEnemiesPool = World.GetPool<HasEnemies>();
                    var levelPool = World.GetPool<LevelComp>();
                    var popupDamageTextPool = World.GetPool<PopupDamageTextComp>();
                    var levelChangedPool = EventWorld.GetPool<LevelChangedEvent>();
                    var hasAbilities = World.GetPool<HasAbilities>();
                    var abilityPool = World.GetPool<AbilityComp>();

                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    ref var playerLevelComp = ref levelPool.Get(PlayerEntity);
                    ref var enemyLevelComp = ref levelPool.Get(unpackedEnemyEntity);
                    ref var playerComp = ref playerPool.Get(PlayerEntity);
                    ref var abilities = ref hasAbilities.Get(PlayerEntity);

                    enemyComp.EnemyState = EnemyState.Chase;
                    enemyComp.Agent.SetDestination(playerComp.Transform.position);
                    enemyComp.CurrentChaseTime += Ts.DeltaTime;

                    var targetDamage = DamageEnemy(playerLevelComp, ref enemyRpgComp);

                    foreach (var abilityPacked in abilities.Entities)
                    {
                        if (abilityPacked.Unpack(World, out var unpackedAbilityEntity))
                        {
                            if (unpackedAbilityEntity == SkillIdx)
                            {
                                var releasedEffectPool = World.GetPool<ReleasedStatusEffectComp>();
                                ref var abilityComp = ref abilityPool.Get(unpackedAbilityEntity);

                                StatusEffectObject effectObject = null;
                                switch (abilityComp.StatusEffect.statusEffectType)
                                {
                                    case FireStatusEffect type:
                                        effectObject = Ps.FireStatusEffectPool.Get();
                                        break;
                                    case IceStatusEffect type:
                                        effectObject = Ps.IceStatusEffectPool.Get();
                                        break;
                                }
                                
                                var effectEntity = World.NewEntity();
                                ref var releasedEffect = ref releasedEffectPool.Add(effectEntity);

                                releasedEffect.StatusOwner = PlayerEntity;
                                releasedEffect.statusEffectObject = effectObject;
                                
                                effectObject.SetWorld(World, PlayerEntity, effectEntity, Sd, Ts, Ps, Cf);
                                effectObject.Applying(enemyView, abilityComp.StatusEffect);
                            }
                        }
                    }

                    ShowPopupDamage(popupDamageTextPool, targetDamage, enemyComp);

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
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                        hasEnemyComp.Entities.RemoveAll(entityPacked =>
                                            entityPacked.Unpack(World, out var entity) &&
                                            entity == unpackedEnemyEntity);
                            }
                            
                            playerComp.GoldAmount += Random.Range(enemyComp.MinCoinsAward, enemyComp.MaxCoinsAward + 1);
                            
                            ref var levelChangedComp = ref levelChangedPool.Add(EventWorld.NewEntity());
                            levelChangedComp.NewExperience = enemyLevelComp.ExperienceToNextLevel / enemyLevelComp.AwardExperienceDiv;

                            enemyPool.Del(unpackedEnemyEntity);
                            enemyRpgPool.Del(unpackedEnemyEntity);
                        }
                    }

                    DestroySpell();
                }
        }
        
        protected virtual void ShowPopupDamage(EcsPool<PopupDamageTextComp> popupDamageTextPool, float targetDamage, EnemyComp enemyComp)
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
        
        protected virtual float DamageEnemy(LevelComp levelComp, ref RpgComp enemyRpgComp)
        {
            var crit = Random.Range(-10, 1) + levelComp.Luck;

            var defaultDamageCrit = crit switch
            {
                > 0 => 1.5f,
                < 0 => 1,
                _ => 2.5f
            };

            var targetDamage = damage * (levelComp.PAtk / 100 + 1) * defaultDamageCrit;

            enemyRpgComp.Health -= targetDamage;
            return targetDamage;
        }

        protected virtual void DestroySpell()
        {
            if (AbilityIdx.Unpack(World, out var unpackedEntity))
            {
                var releasedAbilityPool = World.GetPool<ReleasedAbilityComp>();

                ref var releasedAbilityComp = ref releasedAbilityPool.Get(unpackedEntity);

                Ps.FireBallSpellPool.Return(releasedAbilityComp.AbilityObject);
                releasedAbilityPool.Del(unpackedEntity);
            }
        }

        public override void Cast(AbilityComp ability, int entity)
        {
            var playerPool = World.GetPool<PlayerComp>();

            ref var playerComp = ref playerPool.Get(entity);

            var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            var ray = Sd.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
            Vector3 abilityDirection;

            if (Physics.Raycast(ray, out var hitInfo, ((BallAbility)ability.AbilityType).Distance))
                abilityDirection = hitInfo.point;
            else
                abilityDirection = ray.GetPoint(((BallAbility)ability.AbilityType).Distance);

            var journeyLenght = Vector3.Distance(playerComp.Transform.position + playerComp.Transform.forward,
                abilityDirection);

            var newPostion = playerComp.Transform.position + playerComp.Transform.forward * 3;

            transform.position = newPostion;
            
            damage = ((BallAbility)ability.AbilityType).Damage;
            startTime = Ts.Time;
            startDirection = playerComp.Transform.position + playerComp.Transform.forward * 2;
            direction = journeyLenght;
            endDirection = abilityDirection;
            speed = ((BallAbility)ability.AbilityType).Speed;
        }
    }
}