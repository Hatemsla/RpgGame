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
    public sealed class EnemyInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp>> _zoneFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<HasEnemies> _hasEnemiesPool = default;
        private readonly EcsPoolInject<LevelComp> _levelPool = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        
        private readonly EcsWorldInject _world = default;
        
        public void Init(IEcsSystems systems)
        {
            _ps.Value.EnemyPool = new PoolBase<EnemyView>(Preload, GetAction, ReturnAction, 0);
            
            foreach (var zoneEntity in _zoneFilter.Value)
            {
                ref var zoneComp = ref _zoneFilter.Pools.Inc1.Get(zoneEntity);
                var zonePackedEntity = _world.Value.PackEntity(zoneEntity);
                ref var hasEnemies = ref _hasEnemiesPool.Value.Add(zoneEntity);
                
                for (var i = 0; i < zoneComp.ZoneView.enemiesCount; i++)
                {
                    var enemyEntity = _world.Value.NewEntity();

                    ref var enemy = ref _enemyPool.Value.Add(enemyEntity);
                    ref var rpg = ref _rpgPool.Value.Add(enemyEntity);
                    ref var level = ref _levelPool.Value.Add(enemyEntity);

                    var randomEnemy = Random.Range(0, zoneComp.ZoneView.enemiesType.Count - 1);
                    var randomEnemyType = zoneComp.ZoneView.enemiesType[randomEnemy];
                    var enemyPrefab = randomEnemyType.enemyView;

                    _ps.Value.EnemyPool.Add(Object.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity));
                    var enemyView = _ps.Value.EnemyPool.Get();

                    var enemyIndex = _cf.Value.enemyConfiguration.enemiesData.IndexOf(randomEnemyType);
                    
                    enemy.EnemyIndex = enemyIndex;
                    enemy.EnemyName = randomEnemyType.enemyName;
                    enemy.Transform = enemyView.transform;
                    enemy.EnemyView = enemyView;
                    enemy.Agent = enemyView.GetComponent<NavMeshAgent>();
                    enemy.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                    enemy.Transform.SetParent(zoneComp.ZoneView.transform);
                    enemy.Transform.localPosition = zoneComp.ZoneView
                        .targets[Random.Range(0, zoneComp.ZoneView.targets.Count - 1)].transform.localPosition;
                    enemy.EnemyState = EnemyState.Patrol;
                    enemy.Agent.enabled = true;
                    enemy.MinDamage = randomEnemyType.minDamage;
                    enemy.MaxDamage = randomEnemyType.maxDamage;
                    enemy.AttackDelay = randomEnemyType.attackDelay;
                    enemy.EnemyView.currentAttackDelay = enemy.AttackDelay;
                    
                    rpg.Health = randomEnemyType.health;
                    rpg.Stamina = randomEnemyType.stamina;
                    rpg.Mana = randomEnemyType.mana;
                    rpg.CanDash = true;
                    rpg.CanJump = true;
                    rpg.CanRun = true;
                    
                    level.Level = Random.Range(zoneComp.ZoneView.minEnemyLevel, zoneComp.ZoneView.maxEnemyLevel + 1);
                    level.Experience = _cf.Value.enemyConfiguration.enemiesData[enemyIndex].startExperience;
                    level.ExperienceToNextLevel = _cf.Value.enemyConfiguration.enemiesData[enemyIndex].experienceToNextLevel[level.Level - 1];
                    
                    var enemyPackedEntity = _world.Value.PackEntity(enemyEntity);

                    enemyView.EnemyPackedIdx = enemyPackedEntity;
                    enemyView.ZonePackedIdx = zonePackedEntity;
                    enemyView.SetWorld(_world.Value, enemyEntity);
                    
                    hasEnemies.Entities.Add(enemyPackedEntity);
                }
            }
        }
        
        private EnemyView Preload() => Object.Instantiate(_cf.Value.enemyConfiguration.enemiesData[0].enemyView, 
            Vector3.zero, Quaternion.identity);

        private void GetAction(EnemyView spellObject) => spellObject.gameObject.SetActive(true);
        private void ReturnAction(EnemyView spellObject) => spellObject.gameObject.SetActive(false);
    }
}