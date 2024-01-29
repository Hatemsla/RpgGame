using System.Globalization;
using Leopotam.EcsLite;
using UnityEngine;
using World.Ability.AbilitiesTypes;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;
using World.UI.PopupText;

namespace World.Ability.AbilitiesObjects
{
    public class BallAbilityObject : DirectionalAbilityObject
    {
        [HideInInspector] public float speed;
        [HideInInspector] public float startTime;

        private void Update()
        {
            var distanceCovered = (Ts.Time - startTime) * speed;
            var journeyFraction = distanceCovered / direction;
            transform.position = Vector3.Lerp(startDirection, endDirection, journeyFraction);

            if (journeyFraction >= 1.0f) DestroySpell();
        }

        private void OnCollisionEnter(Collision other)
        {
            var enemyView = other.gameObject.GetComponent<EnemyView>();

            if (enemyView)
                if (enemyView.EnemyPackedIdx.Unpack(World, out var unpackedEnemyEntity))
                {
                    var enemyPool = World.GetPool<EnemyComp>();
                    var enemyRpgPool = World.GetPool<RpgComp>();
                    var hasEnemiesPool = World.GetPool<HasEnemies>();
                    var levelPool = World.GetPool<LevelComp>();
                    var popupDamageTextPool = World.GetPool<PopupDamageTextComp>();

                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    ref var levelComp = ref levelPool.Get(PlayerEntity);

                    var targetDamage = damage * (levelComp.MAtk / 100 + 1);
                    
                    enemyRpgComp.Health -= targetDamage;
                    
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

                            enemyPool.Del(unpackedEnemyEntity);
                            enemyRpgPool.Del(unpackedEnemyEntity);
                        }
                    }

                    DestroySpell();
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

        private void DestroySpell()
        {
            if (AbilityIdx.Unpack(World, out var unpackedEntity))
            {
                var releasedAbilityPool = World.GetPool<ReleasedAbilityComp>();

                ref var releasedAbilityComp = ref releasedAbilityPool.Get(unpackedEntity);

                Ps.SpellPool.Return(releasedAbilityComp.abilityObject);
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

            if (Physics.Raycast(ray, out var hitInfo, ((BallAbility)ability.abilityType).Distance))
                abilityDirection = hitInfo.point;
            else
                abilityDirection = ray.GetPoint(((BallAbility)ability.abilityType).Distance);

            var journeyLenght = Vector3.Distance(playerComp.Transform.position + playerComp.Transform.forward,
                abilityDirection);

            transform.position = playerComp.Transform.position + playerComp.Transform.forward;

            damage = ((BallAbility)ability.abilityType).Damage;
            startTime = Ts.Time;
            startDirection = playerComp.Transform.position + playerComp.Transform.forward;
            direction = journeyLenght;
            endDirection = abilityDirection;
            speed = ((BallAbility)ability.abilityType).Speed;
        }
    }
}