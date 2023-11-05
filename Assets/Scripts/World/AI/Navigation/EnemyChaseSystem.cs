using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Player;

namespace World.AI.Navigation
{
    public sealed class EnemyChaseSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _playerFilter = default;
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
                    
                    if(enemyComp.EnemyState == EnemyState.Attack) continue;

                    foreach (var playerEntity in _playerFilter.Value)
                    {
                        ref var playerComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);
                        ref var rpgComp = ref _playerFilter.Pools.Inc2.Get(playerEntity);

                        if (rpgComp.IsDead)
                        {
                            enemyComp.EnemyState = EnemyState.Patrol;
                            continue;
                        }
                        
                        var distanceToPlayer = Vector3.Distance(playerComp.Transform.position,
                            enemyComp.Agent.transform.position);
                    
                        if (Math.Abs(distanceToPlayer) < 7f && enemyComp.EnemyState != EnemyState.Chase)
                        {
                            enemyComp.EnemyState = EnemyState.Chase;
                        }
                        else if(enemyComp.EnemyState == EnemyState.Chase && Math.Abs(distanceToPlayer) >= 7f)
                        {
                            enemyComp.EnemyState = EnemyState.Patrol;
                        }

                        if(enemyComp.EnemyState == EnemyState.Chase)
                            enemyComp.Agent.SetDestination(playerComp.Transform.position);
                    }
                }
            }
        }
    }
}