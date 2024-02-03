using System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Player;
using World.RPG;

namespace World.AI.Navigation
{
    public sealed class EnemyChaseSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ZoneComp, HasEnemies>> _zoneFilter = default;
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _playerFilter = default;
        private readonly EcsPoolInject<EnemyComp> _enemyPool = default;
        private readonly EcsPoolInject<AnimationComp> _animationPool = default;

        private readonly EcsCustomInject<TimeService> _ts = default;

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

                        if (enemyComp.EnemyState == EnemyState.Attack) continue;

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

                            if (enemyComp.CurrentChaseTime >= enemyComp.ChaseTime)
                            {
                                if (enemyComp.CurrentUnChaseTime <= enemyComp.UnChaseTime)
                                {
                                    enemyComp.CurrentUnChaseTime += _ts.Value.DeltaTime;
                                    enemyComp.EnemyState = EnemyState.Patrol;
                                    continue;
                                }

                                enemyComp.CurrentChaseTime = 0;
                                enemyComp.CurrentUnChaseTime = 0;
                            }
                            
                            if (Math.Abs(distanceToPlayer) < enemyComp.ChaseDistance && enemyComp.EnemyState != EnemyState.Chase)
                            {
                                enemyComp.EnemyState = EnemyState.Chase;
                            }
                            else if (enemyComp is { EnemyState: EnemyState.Chase, CurrentChaseTime: 0 } && Math.Abs(distanceToPlayer) >= enemyComp.ChaseDistance)
                            {
                                enemyComp.EnemyState = EnemyState.Patrol;
                            }
                            
                            if (enemyComp.EnemyState == EnemyState.Chase &&
                                enemyComp.EnemyView.gameObject.activeInHierarchy)
                            {
                                if (distanceToPlayer > enemyComp.MinDistanceToPlayer)
                                {
                                    enemyComp.Agent.isStopped = false;
                                    animationComp.Animator.SetFloat(MoveX, 1f);
                                    enemyComp.Agent.SetDestination(playerComp.Transform.position);
                                }
                                else
                                {
                                    enemyComp.Agent.isStopped = true;
                                    animationComp.Animator.SetFloat(MoveX, 0f);
                                }

                                if (enemyComp.CurrentChaseTime <= enemyComp.ChaseTime)
                                {
                                    enemyComp.CurrentChaseTime += _ts.Value.DeltaTime;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}