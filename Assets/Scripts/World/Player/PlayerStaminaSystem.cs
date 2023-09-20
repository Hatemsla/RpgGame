using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
                
                var sprintEndurance = rpg.Stamina - _cf.Value.playerConfiguration.sprintEndurance * _ts.Value.DeltaTime;
                rpg.CanRun = sprintEndurance > 0;
                if (input.Sprint)
                    if (rpg.CanRun)
                        rpg.Stamina = sprintEndurance;
                
                var dashEndurance = rpg.Stamina - _cf.Value.playerConfiguration.dashEndurance;
                rpg.CanDash = dashEndurance > 0;
                if (input.Dash)
                    if (rpg.CanDash)
                        rpg.Stamina = dashEndurance;
                
                var jumpEndurance = rpg.Stamina - _cf.Value.playerConfiguration.jumpEndurance;
                rpg.CanJump = jumpEndurance > 0;
                if (input.Jump && player.Grounded)
                    if (rpg.CanJump)
                        rpg.Stamina = jumpEndurance;

                rpg.Stamina =
                    Mathf.Clamp(rpg.Stamina + _cf.Value.playerConfiguration.staminaRecovery * _ts.Value.DeltaTime, 0,
                        _cf.Value.playerConfiguration.stamina);

                var targetStaminaValue = Utils.Utils.Map(rpg.Stamina, 0, _cf.Value.playerConfiguration.stamina, 0, 1);
                _staminaBar.value = Mathf.Lerp(_staminaBar.value, targetStaminaValue, _cf.Value.uiConfiguration.hsmBarsChangeRate * _ts.Value.DeltaTime);
            }
        }
    }
}