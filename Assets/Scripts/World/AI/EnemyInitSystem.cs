using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using World.AI.Navigation;
using World.Configurations;
using World.Player;

namespace World.AI
{
    public sealed class EnemyInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp>> _zoneFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<HasEnemies> _hasEnemiesPool = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private readonly EcsWorldInject _world = default;
        
        public void Init(IEcsSystems systems)
        {
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

                    var randomEnemy = Random.Range(0, zoneComp.ZoneView.enemiesType.Count - 1);
                    var enemyPrefab = zoneComp.ZoneView.enemiesType[randomEnemy].enemyView;

                    var enemyView = Object.Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
                    
                    enemy.Transform = enemyView.transform;
                    enemy.EnemyView = enemyView;
                    enemy.Agent = enemyView.GetComponent<NavMeshAgent>();
                    enemy.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                    enemy.Transform.SetParent(zoneComp.ZoneView.transform);
                    enemy.Transform.localPosition = Vector3.zero;
                    enemy.EnemyState = EnemyState.Patrol;
                    enemy.Agent.enabled = true;
                    enemy.MinDamage = zoneComp.ZoneView.enemiesType[randomEnemy].minDamage;
                    enemy.MaxDamage = zoneComp.ZoneView.enemiesType[randomEnemy].maxDamage;
                    enemy.AttackDelay = zoneComp.ZoneView.enemiesType[randomEnemy].attackDelay;
                    enemy.EnemyView.currentAttackDelay = enemy.AttackDelay;

                    var randomEnemyType = zoneComp.ZoneView.enemiesType[randomEnemy];
                    rpg.Health = randomEnemyType.health;
                    rpg.Stamina = randomEnemyType.stamina;
                    rpg.Mana = randomEnemyType.mana;
                    rpg.CanDash = true;
                    rpg.CanJump = true;
                    rpg.CanRun = true;
                    
                    var enemyPackedEntity = _world.Value.PackEntity(enemyEntity);

                    enemyView.EnemyPacked = enemyPackedEntity;
                    enemyView.ZonePacked = zonePackedEntity;
                    enemyView.SetWorld(_world.Value, enemyEntity);
                    
                    hasEnemies.Entities.Add(enemyPackedEntity);
                }
            }
        }
    }
}