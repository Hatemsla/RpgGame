using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Player.Events;
using World.RPG;

namespace World.Player
{
    public class PlayerWaitForEndDeathAnimationSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<RpgComp, PlayerComp>> _playerFilter = default;
        private readonly EcsFilterInject<Inc<DeathAnimationEvent>> _deathAnimationFlter = Idents.Worlds.Events;

        private readonly EcsCustomInject<TimeService> _ts = default;
        
        private float _deathAnimationDelay = 2f;
        private float _currentDeathAnimationDelay;
        
        private bool _isDeathAnimationDelayed = true;
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _deathAnimationFlter.Value)
            {
                _isDeathAnimationDelayed = false;
                _currentDeathAnimationDelay = _deathAnimationDelay;
            }

            if (!_isDeathAnimationDelayed)
            {
                _currentDeathAnimationDelay -= _ts.Value.DeltaTime; 
                Debug.Log(_currentDeathAnimationDelay);
                if (_currentDeathAnimationDelay <= 0)
                {
                    foreach (var entity in _playerFilter.Value)
                    {
                        ref var rpgComp = ref _playerFilter.Pools.Inc1.Get(entity);

                        rpgComp.IsDead = false;
                        _isDeathAnimationDelayed = true;
                    }
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            _currentDeathAnimationDelay = _deathAnimationDelay;
        }
    }
}