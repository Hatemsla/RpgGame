using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Utils;
using World.RPG;

namespace World.Player.Weapons
{
    public sealed class OneMeleeAttackDelaySystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _playerFilter = default;
        private readonly EcsFilterInject<Inc<OneHandedMeleeAttackEvent>> _oneHandedMeleePool = Idents.Worlds.Events;
        private readonly EcsCustomInject<TimeService> _ts = default;
        
        private float _oneHandedAttackDelay = 0.7f;
        private float _currentOneHandedAttackDelay;
        private bool _isOneHandedAttackDelayed = true;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _oneHandedMeleePool.Value)
            {
                _isOneHandedAttackDelayed = false;
                _currentOneHandedAttackDelay = _oneHandedAttackDelay;
                foreach (var playerEntity in _playerFilter.Value)
                {
                    ref var playerComp = ref _playerFilter.Pools.Inc1.Get(playerEntity);

                    playerComp.CanMove = false;
                }
            }
            
            if (!_isOneHandedAttackDelayed)
            {
                _currentOneHandedAttackDelay -= _ts.Value.DeltaTime;
                if (_currentOneHandedAttackDelay <= 0)
                {
                    foreach (var entity in _playerFilter.Value)
                    {
                        ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);

                        playerComp.CanMove = true;
                        _isOneHandedAttackDelayed = true;
                    }
                }
            }
        }

        public void Init(IEcsSystems systems)
        {
            _currentOneHandedAttackDelay = _oneHandedAttackDelay;
        }
    }
}