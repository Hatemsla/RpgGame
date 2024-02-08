using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using Utils.ObjectsPool;
using World.AI.Navigation;
using World.Configurations;
using World.Player;
using World.RPG;

namespace World.AI
{
    public sealed class EnemyRespawnSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<LevelComp> _levelPool = default;
        private readonly EcsPoolInject<AnimationComp> _animationPool = default;

        private readonly EcsCustomInject<PoolService> _ps = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;

        private readonly EcsWorldInject _world = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var zoneEntity in _zoneFilter.Value)
            {
                ref var zoneComp = ref _zoneFilter.Pools.Inc1.Get(zoneEntity);
                var zonePackedEntity = _world.Value.PackEntity(zoneEntity);
                ref var hasEnemiesComp = ref _zoneFilter.Pools.Inc2.Get(zoneEntity);
                var count = 0;

                foreach (var packedEntity in hasEnemiesComp.Entities)
                    if (packedEntity.Unpack(_world.Value, out _))
                        count++;

                for (var i = 0; i < zoneComp.ZoneView.enemiesCount - count; i++)
                {
                    var enemyEntity = _world.Value.NewEntity();
                    
                    ref var enemyComp = ref _enemyPool.Value.Add(enemyEntity);
                    ref var rpgComp = ref _rpgPool.Value.Add(enemyEntity);
                    ref var levelComp = ref _levelPool.Value.Add(enemyEntity);
                    ref var animationComp = ref _animationPool.Value.Add(enemyEntity);
                    
                    var randomEnemy = Random.Range(0, zoneComp.ZoneView.enemiesType.Count - 1);
                    var randomEnemyType = zoneComp.ZoneView.enemiesType[randomEnemy];
                    
                    var enemyView = _ps.Value.EnemyPool.Get();
                    enemyView.gameObject.SetActive(false);
                    
                    var enemyIndex = _cf.Value.enemyConfiguration.enemiesData.IndexOf(randomEnemyType);
                    
                    enemyComp.EnemyIndex = enemyIndex;
                    enemyComp.EnemyName = randomEnemyType.enemyName;
                    enemyComp.Transform = enemyView.transform;
                    enemyComp.EnemyView = enemyView;
                    enemyComp.ChaseDistance = randomEnemyType.chaseDistance;
                    enemyComp.ChaseTime = randomEnemyType.chaseTime;
                    enemyComp.UnChaseTime = randomEnemyType.unChaseTime;
                    enemyComp.CurrentChaseTime = 0;
                    enemyComp.MinDistanceToPlayer = randomEnemyType.minDistanceToPlayer;
                    enemyComp.Agent = enemyView.GetComponent<NavMeshAgent>();
                    enemyComp.Agent.enabled = true;
                    enemyComp.Agent.isStopped = false;
                    
                    enemyComp.WalkSpeed = randomEnemyType.walkSpeed;
                    enemyComp.RunSpeed = randomEnemyType.runSpeed;
                    enemyComp.AngularWalkSpeed = randomEnemyType.angularWalkSpeed;
                    enemyComp.AngularRunSpeed = randomEnemyType.angularRunSpeed;
                    
                    enemyComp.Agent.speed = randomEnemyType.walkSpeed;
                    enemyComp.Agent.angularSpeed = randomEnemyType.angularWalkSpeed;
                    enemyComp.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                    enemyComp.Transform.SetParent(zoneComp.ZoneView.transform);
                    enemyComp.Transform.localPosition = zoneComp.ZoneView
                        .targets[Random.Range(0, zoneComp.ZoneView.targets.Count - 1)].transform.localPosition;
                    enemyComp.EnemyState = EnemyState.Patrol;
                    enemyComp.MinDamage = randomEnemyType.minDamage;
                    enemyComp.MaxDamage = randomEnemyType.maxDamage;
                    enemyComp.AttackDelay = randomEnemyType.attackDelay;
                    enemyComp.EnemyView.currentAttackDelay = enemyComp.AttackDelay;
                    
                    enemyComp.MinCoinsAward = randomEnemyType.minCoinsAward;
                    enemyComp.MaxCoinsAward = randomEnemyType.maxCoinsAward;
                    
                    rpgComp.Health = randomEnemyType.health;
                    rpgComp.Stamina = randomEnemyType.stamina;
                    rpgComp.Mana = randomEnemyType.mana;
                    rpgComp.CanDash = true;
                    rpgComp.CanJump = true;
                    rpgComp.CanRun = true;
                    
                    animationComp.Animator = enemyComp.EnemyView.GetComponentInChildren<Animator>();

                    levelComp.AwardExperienceDiv = randomEnemyType.awardExperienceDiv;
                    levelComp.Level = Random.Range(zoneComp.ZoneView.minEnemyLevel, zoneComp.ZoneView.maxEnemyLevel + 1);
                    levelComp.Experience = _cf.Value.enemyConfiguration.enemiesData[enemyIndex].startExperience;
                    levelComp.ExperienceToNextLevel = _cf.Value.enemyConfiguration.enemiesData[enemyIndex].experienceToNextLevel[levelComp.Level - 1];
                    
                    var enemyPackedEntity = _world.Value.PackEntity(enemyEntity);

                    enemyView.EnemyPackedIdx = enemyPackedEntity;
                    enemyView.ZonePackedIdx = zonePackedEntity;
                    enemyView.SetWorld(_world.Value, enemyEntity);
                    _sd.Value.StartCoroutine(enemyView.Respawn(randomEnemyType.respawnDelay));
                    
                    hasEnemiesComp.Entities.Add(enemyPackedEntity);
                }
            }
        }
    }
}