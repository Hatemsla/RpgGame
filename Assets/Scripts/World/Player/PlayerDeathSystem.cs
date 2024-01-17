using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Player.Events;
using World.RPG;

namespace World.Player
{
    public class PlayerDeathSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp, AnimationComp>> _player = default;
        private readonly EcsPoolInject<DeathAnimationEvent> _deathAnimationPool = Idents.Worlds.Events;
        
        private readonly EcsWorldInject _eventsWorld = Idents.Worlds.Events;
        
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        private bool _isEnded;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var playerComp = ref _player.Pools.Inc1.Get(entity);
                ref var rpgComp = ref _player.Pools.Inc2.Get(entity);
                ref var animationComp = ref _player.Pools.Inc3.Get(entity);

                if (rpgComp.IsDead)
                {
                    if (rpgComp.Health > 0)
                    {
                        if (!_isEnded)
                        {
                            animationComp.Animator.SetBool(IsDead, false);
                            ref var deathComp = ref _deathAnimationPool.Value.Add(_eventsWorld.Value.NewEntity());
                            _isEnded = true;
                        }
                    }
                }
                else
                {
                    if (rpgComp.Health < 0)
                    {
                        _isEnded = false;
                        rpgComp.IsDead = true;
                        animationComp.Animator.SetBool(IsDead, true);
                    }
                }
            }
        }
    }
}