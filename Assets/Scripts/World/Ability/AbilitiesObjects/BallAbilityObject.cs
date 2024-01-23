using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;
using World.Ability.AbilitiesTypes;
using World.AI;
using World.AI.Navigation;
using World.Player;
using World.RPG;

namespace World.Ability.AbilitiesObjects
{
    public class BallAbilityObject : DirectionalAbilityObject
    {
        [HideInInspector] public float speed;
        [HideInInspector] public float startTime;
        
        private EcsWorld world;
        private EcsPool<PlayerComp> player;
        private EcsPool<ReleasedAbilityComp> releasedAbilityPool;
        
        private SceneData sd;
        private TimeService ts;
        private PoolService ps;

        private void Update()
        {
            var distanceCovered = (ts.Time - startTime) * speed;
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
                if (enemyView.EnemyPackedIdx.Unpack(world, out var unpackedEnemyEntity))
                {
                    var enemyPool = world.GetPool<EnemyComp>();
                    var enemyRpgPool = world.GetPool<RpgComp>();
                    var hasEnemiesPool = world.GetPool<HasEnemies>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    
                    enemyRpgComp.Health -= damage;

                    if (enemyRpgComp.Health <= 0)
                    {
                        ps.EnemyPool.Return(enemyComp.EnemyView);
                        
                        enemyPool.Del(unpackedEnemyEntity);
                        enemyRpgPool.Del(unpackedEnemyEntity);
                        ps.EnemyPool.Return(enemyComp.EnemyView);

                        if (enemyView.ZonePackedIdx.Unpack(world, out var unpackedZoneEntity))
                        {
                            ref var hasEnemyComp = ref hasEnemiesPool.Get(unpackedZoneEntity);
                            Debug.Log(hasEnemyComp.Entities.Count);

                            for (var index = 0; index < hasEnemyComp.Entities.Count; index++)
                            {
                                var hasEnemyEntityPacked = hasEnemyComp.Entities[index];
                                if (hasEnemyEntityPacked.Unpack(world, out var unpackedHasEnemyEntity))
                                {
                                    if (unpackedHasEnemyEntity == unpackedEnemyEntity)
                                    {
                                        // hasEnemyComp.Entities[index] = default;
                                        // hasEnemyComp.Entities.Remove(hasEnemyEntityPacked);
                                        hasEnemyComp.Entities.RemoveAll(entityPacked => entityPacked.Unpack(world, out var entity) && entity == unpackedEnemyEntity);
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
            if (AbilityIdx.Unpack(world, out var unpackedEntity))
            {
                ref var spell = ref releasedAbilityPool.Get(unpackedEntity);
                
                ps.SpellPool.Return(spell.abilityObject);
                releasedAbilityPool.Del(unpackedEntity);
            }
        }

        public override void Cast(AbilityComp ability, int entity)
        {
            var player = _player.Get(entity);

            var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            var ray = _sd.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
            Vector3 abilityDirection;

            if (Physics.Raycast(ray, out var hitInfo, ((BallAbility)ability.abilityType).Distance))
                abilityDirection = hitInfo.point;
            else
                abilityDirection = ray.GetPoint(((BallAbility)ability.abilityType).Distance);

            var journeyLenght = Vector3.Distance(player.Transform.position + player.Transform.forward,
                abilityDirection);
            var startTime = _ts.Time;

            var abilityObject = _ps.SpellPool.Get();
            abilityObject.transform.position = player.Transform.position + player.Transform.forward;

            var abilityEntity = _world.NewEntity();
            var abilityPackedEntity = _world.PackEntity(abilityEntity);
            ref var releasedAbility = ref _releasedAbilityPool.Add(abilityEntity);

            releasedAbility.abilityObject = abilityObject;
            releasedAbility.spellOwner = entity;

            ((BallAbilityObject)abilityObject).damage = ((BallAbility)ability.abilityType).Damage;
            ((BallAbilityObject)abilityObject).startTime = startTime;
            ((BallAbilityObject)abilityObject).startDirection = player.Transform.position + player.Transform.forward;
            ((BallAbilityObject)abilityObject).direction = journeyLenght;
            ((BallAbilityObject)abilityObject).endDirection = abilityDirection;  
            ((BallAbilityObject)abilityObject).speed = ((BallAbility)ability.abilityType).Speed;

            ((BallAbilityObject)abilityObject).world = _world;
            ((BallAbilityObject)abilityObject).ts = _ts;
            ((BallAbilityObject)abilityObject).ps = _ps;
            ((BallAbilityObject)abilityObject).sd = _sd;
            ((BallAbilityObject)abilityObject).player = this.player;
            ((BallAbilityObject)abilityObject).releasedAbilityPool = _releasedAbilityPool;

            abilityObject.AbilityIdx = abilityPackedEntity;
        }
    }
}