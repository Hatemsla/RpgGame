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
                    
                    ref var enemy = ref _enemyPool.Value.Add(enemyEntity);
                    ref var rpg = ref _rpgPool.Value.Add(enemyEntity);
                    
                    var randomEnemy = Random.Range(0, zoneComp.ZoneView.enemiesType.Count - 1);
                    var randomEnemyType = zoneComp.ZoneView.enemiesType[randomEnemy];
                    
                    var enemyView = _ps.Value.EnemyPool.Get();
                    enemyView.gameObject.SetActive(false);
                    
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