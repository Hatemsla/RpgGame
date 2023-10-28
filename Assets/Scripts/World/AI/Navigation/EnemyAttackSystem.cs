using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Player;

namespace World.AI.Navigation
{
    public sealed class EnemyAttackSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;

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
                    
                    foreach (var playerEntity in _playerFilter.Value)
                    {
                        ref var playerComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);
                        
                        var distanceToPlayer = Vector3.Distance(playerComp.Transform.position,
                            enemyComp.Agent.transform.position);
                    
                        if (Math.Abs(distanceToPlayer) < 3f && enemyComp.EnemyState != EnemyState.Attack)
                        {
                            enemyComp.EnemyState = EnemyState.Attack;
                        }
                        else if(enemyComp.EnemyState == EnemyState.Attack && Math.Abs(distanceToPlayer) >= 3f)
                        {
                            enemyComp.EnemyState = EnemyState.Chase;
                        }

                        if (enemyComp.EnemyState == EnemyState.Attack)
                        {
                            enemyComp.Agent.SetDestination(playerComp.Transform.position);
                        }
                    }
                }
            }
        }
    }
}