using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using Random = UnityEngine.Random;

namespace World.AI.Navigation
{
    public sealed class EnemyPatrolSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private readonly EcsWorldInject _world = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var zoneEntity in _zoneFilter.Value)
            {
                ref var zoneComp = ref _zoneFilter.Pools.Inc1.Get(zoneEntity);
                ref var hasEnemiesComp = ref _zoneFilter.Pools.Inc2.Get(zoneEntity);

                foreach (var packedEnemy in hasEnemiesComp.Entities)
                {
                    if (packedEnemy.Unpack(_world.Value, out var unpackedEnemy))
                    {

                        ref var enemyComp = ref _enemyPool.Value.Get(unpackedEnemy);

                        if (enemyComp.EnemyState != EnemyState.Patrol) continue;

                        var distance = Vector3.Distance(enemyComp.Agent.transform.position,
                            enemyComp.Agent.pathEndPosition);

                        if (Math.Abs(distance - 1f) < _cf.Value.enemyConfiguration.targetError)
                        {
                            enemyComp.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                        }

                        enemyComp.Agent.SetDestination(zoneComp.ZoneView.targets[enemyComp.TargetIndex].transform
                            .position);
                    }
                }
            }
        }
    }
}