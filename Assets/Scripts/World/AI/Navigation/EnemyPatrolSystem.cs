using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.Player;
using Random = UnityEngine.Random;

namespace World.AI.Navigation
{
    public sealed class EnemyPatrolSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        private readonly EcsPoolInject<AnimationComp> _animationPool = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private readonly EcsWorldInject _world = default;
        private static readonly int MoveX = Animator.StringToHash("MoveX");

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
                        ref var animationComp = ref _animationPool.Value.Get(unpackedEnemy);

                        if (enemyComp.EnemyState != EnemyState.Patrol) continue;

                        var distance = Vector3.Distance(enemyComp.Agent.transform.position,
                            enemyComp.Agent.pathEndPosition);

                        if (Math.Abs(distance - 1f) < _cf.Value.enemyConfiguration.targetError)
                        {
                            enemyComp.TargetIndex = Random.Range(0, zoneComp.ZoneView.targets.Count);
                        }

                        if (enemyComp.EnemyView.gameObject.activeInHierarchy)
                        {
                            enemyComp.Agent.isStopped = false;
                            enemyComp.Agent.speed = enemyComp.WalkSpeed;
                            enemyComp.Agent.angularSpeed = enemyComp.AngularWalkSpeed;
                            animationComp.Animator.SetFloat(MoveX, 0.5f);
                            enemyComp.Agent.SetDestination(zoneComp.ZoneView.targets[enemyComp.TargetIndex].transform
                                .position);
                        }
                    }
                }
            }
        }
    }
}