using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Player.Events;

namespace World.Player
{
    public class PlayerCameraTransitionSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;
        private readonly EcsFilterInject<Inc<TransitionCameraEvent>> _filter = Idents.Worlds.Events;

        private readonly EcsCustomInject<TimeService> _ts = default;
        
        private float _timeToWait;
        private float _currentWaitTime;
        private bool _isCameraTransited = true;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var transitionComp = ref _filter.Pools.Inc1.Get(entity);

                _currentWaitTime = _currentWaitTime = transitionComp.TimeToWait;
                _isCameraTransited = false;
            }

            if (!_isCameraTransited)
            {
                _currentWaitTime -= _ts.Value.DeltaTime;
                Debug.Log(_currentWaitTime);
                if (_currentWaitTime <= 0)
                {
                    Debug.Log(_currentWaitTime <= 0);
                    foreach (var entity in _playerFilter.Value)
                    {
                        ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);

                        playerComp.CanMove = true;
                        _isCameraTransited = true;
                        Debug.Log("_isCameraTransited");
                    }
                }
            }
        }
    }
}