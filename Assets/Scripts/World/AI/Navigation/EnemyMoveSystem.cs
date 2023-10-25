using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.Player;

namespace World.AI.Navigation
{
    public sealed class EnemyMoveSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsFilterInject<Inc<EnemyComp, RpgComp>, Exc<PlayerComp>> _enemyFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsWorldInject _world = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var zoneEntity in _zoneFilter.Value)
            {
                ref var zoneComp = ref _zoneFilter.Pools.Inc1.Get(zoneEntity);
                ref var hasEnemiesComp = ref _zoneFilter.Pools.Inc2.Get(zoneEntity);

                foreach (var packedEnemy in hasEnemiesComp.Entities)
                {
                    packedEnemy.Unpack(_world.Value, out var unpackedEnemy);

                    ref var enemyComp = ref _enemyPool.Value.Get(unpackedEnemy);

                    var distance = Vector3.Distance(enemyComp.Agent.transform.position,
                        enemyComp.Agent.pathEndPosition);
                    
                    if (Vector3.Distance(enemyComp.Agent.transform.position, enemyComp.Agent.pathEndPosition) == 1f)
                    {
                        enemyComp.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                    }

                    enemyComp.Agent.SetDestination(zoneComp.ZoneView.targets[enemyComp.TargetIndex].transform.position);
                }
                
                // foreach (var enemyEntity in _enemyFilter.Value)
                // {
                //     ref var enemyComp = ref _enemyFilter.Pools.Inc1.Get(enemyEntity);
                //     ref var rpgComp = ref _enemyFilter.Pools.Inc2.Get(enemyEntity);
                //
                //     
                // }
            }
        }
    }
}