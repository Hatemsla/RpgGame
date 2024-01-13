using Leopotam.EcsLite;
using UnityEngine;
using World.Ability.AbilitiesTypes;
using World.AI;
using World.Player;
using World.RPG;

namespace World.Ability.AbilitiesObjects
{
    public class BallAbilityObject : DirectionalAbilityObject
    {
        public TimeService TimeService;
        [HideInInspector] public float speed;
        [HideInInspector] public float startTime;
        
        private void Update()
        {
            var distanceCovered = (TimeService.Time - startTime) * speed;
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
                if (enemyView.EnemyPacked.Unpack(_world, out var unpackedEnemyEntity))
                {
                    var enemyPool = _world.GetPool<EnemyComp>();
                    var enemyRpgPool = _world.GetPool<RpgComp>();
                    
                    ref var enemyComp = ref enemyPool.Get(unpackedEnemyEntity);
                    ref var enemyRpgComp = ref enemyRpgPool.Get(unpackedEnemyEntity);
                    
                    enemyRpgComp.Health -= damage;

                    if (enemyRpgComp.Health <= 0)
                    {
                        PoolService.EnemyPool.Return(enemyComp.EnemyView);
                        
                        enemyPool.Del(unpackedEnemyEntity);
                        enemyRpgPool.Del(unpackedEnemyEntity);
                    }
                    
                    DestroySpell();
                }
            }
        }

        private void DestroySpell()
        {
            if (AbilityIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var spell = ref _releasedAbilityPool.Get(unpackedEntity);
                
                PoolService.SpellPool.Return(spell.abilityObject);
                _releasedAbilityPool.Del(unpackedEntity);
            }
        }

        public override void Cast()
        {
            var player = _player.Pools.Inc1.Get(entity);

            var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            var ray = _sd.Value.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
            Vector3 abilityDirection;

            if (Physics.Raycast(ray, out var hitInfo, ((BallAbility)ability.abilityType).Distance))
                abilityDirection = hitInfo.point;
            else
                abilityDirection = ray.GetPoint(((BallAbility)ability.abilityType).Distance);

            var journeyLenght = Vector3.Distance(player.Transform.position + player.Transform.forward,
                abilityDirection);
            var startTime = _ts.Value.Time;

            var abilityObject = _ps.Value.SpellPool.Get();
            abilityObject.transform.position = player.Transform.position + player.Transform.forward;

            var abilityEntity = _world.Value.NewEntity();
            var abilityPackedEntity = _world.Value.PackEntity(abilityEntity);
            ref var releasedAbility = ref _spell.Value.Add(abilityEntity);

            releasedAbility.abilityObject = abilityObject;
            releasedAbility.spellOwner = entity;

            abilityObject.PoolService = _ps.Value;
            ((BallAbilityObject)abilityObject).TimeService = _ts.Value;

            ((BallAbilityObject)abilityObject).damage = ((BallAbility)ability.abilityType).Damage;
            ((BallAbilityObject)abilityObject).startTime = startTime;
            ((BallAbilityObject)abilityObject).startDirection = player.Transform.position + player.Transform.forward;  
            ((BallAbilityObject)abilityObject).direction = journeyLenght;
            ((BallAbilityObject)abilityObject).endDirection = abilityDirection;  
            ((BallAbilityObject)abilityObject).speed = ((BallAbility)ability.abilityType).Speed;

            abilityObject.AbilityIdx = abilityPackedEntity;
            abilityObject.SetWorld(_world.Value, entity);
        }
    }
}