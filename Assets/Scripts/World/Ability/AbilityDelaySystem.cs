using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using Utils.ObjectsPool;
using World.Player;
using World.RPG;

namespace World.Ability
{
    public class AbilityDelaySystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, RpgComp>> _player = default;

        private readonly EcsPoolInject<AbilityComp> _abilityPool = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilities = default;

        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<PoolService> _ps = default;

        private readonly EcsWorldInject _world = default;

        [EcsUguiNamed(Idents.UI.PlayerAbilityView)]
        private readonly GameObject _abilityView = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _player.Value)
            {
                ref var hasAbilities = ref _hasAbilities.Value.Get(playerEntity);
                ref var rpg = ref _player.Pools.Inc2.Get(playerEntity);

                foreach (var abilityPacked in hasAbilities.Entities)
                {
                    if (abilityPacked.Unpack(_world.Value, out var unpackedEntity))
                    {
                        ref var ability = ref _abilityPool.Value.Get(unpackedEntity);

                        if (ability.currentDelay > 0)
                        {
                            ability.currentDelay -= _ts.Value.DeltaTime;
                        }

                        if (rpg.CastDelay > 0)
                        {
                            rpg.CastDelay -= _ts.Value.DeltaTime;
                        }
                    }
                }
            }
        }
    }
}