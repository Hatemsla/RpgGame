using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils.ObjectsPool;
using World.Ability;
using World.Ability.AbilitiesData;
using World.Configurations;

namespace World.Player
{
    public sealed class PlayerSpellCastSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _player = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        
        private readonly EcsCustomInject<CursorService> _cs = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        private readonly EcsCustomInject<TimeService> _ts = default;

        private readonly EcsPoolInject<SpellComp> _spell = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _player.Value)
            {
                var world = systems.GetWorld();
                ref var player = ref _player.Pools.Inc1.Get(playerEntity);
                ref var input = ref _player.Pools.Inc2.Get(playerEntity);
                ref var rpg = ref _player.Pools.Inc3.Get(playerEntity);

                if (rpg.IsDead || input.FreeCursor || _cs.Value.CursorVisible) return;

                if (EventSystem.current.IsPointerOverGameObject()) return;

                if (input.UseAbility)
                {
                    var abilityData = _cf.Value.abilityConfiguration.abilityDatas[0];

                    if (rpg.Mana >= abilityData.costPoint)
                    {
                        rpg.Mana -= abilityData.costPoint;
                        
                        var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                        var ray = _sd.Value.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
                        Vector3 spellDirection;

                        if (Physics.Raycast(ray, out var hitInfo, ((SpellAbilityData)abilityData.abilityTypeData).distance))
                            spellDirection = hitInfo.point;
                        else
                            spellDirection = ray.GetPoint(((SpellAbilityData)abilityData.abilityTypeData).distance);

                        var journeyLenght = Vector3.Distance(player.Transform.position + player.Transform.forward,
                        spellDirection);
                        var startTime = _ts.Value.Time;

                        var spellObject = _ps.Value.SpellPool.Get();
                        spellObject.transform.position = player.Transform.position + player.Transform.forward;

                        var spellEntity = world.NewEntity();
                        var spellPackedEntity = world.PackEntity(spellEntity);
                        ref var spell = ref _spell.Value.Add(spellEntity);

                        spell.spellObject = spellObject;
                        spell.spellOwner = playerEntity;

                        spellObject.PoolService = _ps.Value;
                        spellObject.TimeService = _ts.Value;

                        spellObject.spellDamage = ((SpellAbilityData)abilityData.abilityTypeData).damage;
                        spellObject.spellTime = startTime;
                        spellObject.spellDirection = journeyLenght;
                        spellObject.spellStart = player.Transform.position + player.Transform.forward;
                        spellObject.spellEnd = spellDirection;
                        spellObject.spellSpeed = ((SpellAbilityData)abilityData.abilityTypeData).speed;

                        spellObject.SpellIdx = spellPackedEntity;
                        spellObject.SetWorld(world, playerEntity);
                    }
                }
            }
        }
    }
}