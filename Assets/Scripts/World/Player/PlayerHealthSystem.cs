using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public class PlayerHealthSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _player = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        [EcsUguiNamed(Idents.UI.HealthBar)]
        private readonly Slider _healthBar = default;

        private bool _isLargeHeight;
        private float _fellDamage;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var rpg = ref _player.Pools.Inc2.Get(entity);

                if (!rpg.IsDead)
                {
                    if (rpg.Health < _cf.Value.playerConfiguration.health)
                        rpg.Health += _cf.Value.playerConfiguration.healthRecovery * _ts.Value.DeltaTime;

                    if (rpg.Health > _cf.Value.playerConfiguration.health)
                        rpg.Health = _cf.Value.playerConfiguration.health;
                }

                var targetHealthValue = Utils.Utils.Map(rpg.Health, 0, _cf.Value.playerConfiguration.health, 0, 1);
                _healthBar.value = Mathf.Lerp(_healthBar.value, targetHealthValue,
                        _cf.Value.uiConfiguration.hsmBarsChangeRate * _ts.Value.DeltaTime);
            }
        }
    }
}