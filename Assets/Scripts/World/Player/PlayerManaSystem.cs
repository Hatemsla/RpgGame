using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace World.Player
{
    public class PlayerManaSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _player = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        [EcsUguiNamed(Idents.UI.ManaBar)]
        private readonly Slider _manaBar = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _player.Value)
            {
                ref var player = ref _player.Pools.Inc1.Get(entity);
                ref var rpg = ref _player.Pools.Inc2.Get(entity);

                if (rpg.Mana < _cf.Value.playerConfiguration.mana)
                    rpg.Mana += _cf.Value.playerConfiguration.manaRecovery * _ts.Value.DeltaTime;

                if (rpg.Mana > _cf.Value.playerConfiguration.mana)
                    rpg.Mana = _cf.Value.playerConfiguration.mana;

                var targetManaValue = Utils.Utils.Map(rpg.Mana, 0, _cf.Value.playerConfiguration.mana, 0, 1);
                _manaBar.value = Mathf.Lerp(_manaBar.value, targetManaValue,
                    _cf.Value.uiConfiguration.hsmBarsChangeRate * _ts.Value.DeltaTime); 
            }
        }
    }
}