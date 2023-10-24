using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using World.Configurations;

namespace World.Player
{
    public class PlayerStaminaSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _player = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        [EcsUguiNamed(Idents.UI.StaminaBar)]
        private readonly Slider _staminaBar = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var input = ref _player.Pools.Inc2.Get(entity);
                ref var rpg = ref _player.Pools.Inc3.Get(entity);

                if (!rpg.IsDead)
                {
                    if (rpg.Stamina < _cf.Value.playerConfiguration.stamina)
                        rpg.Stamina += _cf.Value.playerConfiguration.staminaRecovery * _ts.Value.DeltaTime;

                    if (rpg.Stamina > _cf.Value.playerConfiguration.stamina)
                        rpg.Stamina = _cf.Value.playerConfiguration.stamina;
                }

                var targetStaminaValue = Utils.Utils.Map(rpg.Stamina, 0, _cf.Value.playerConfiguration.stamina, 0, 1);
                _staminaBar.value = Mathf.Lerp(_staminaBar.value, targetStaminaValue, _cf.Value.uiConfiguration.hsmBarsChangeRate * _ts.Value.DeltaTime);
            }
        }
    }
}