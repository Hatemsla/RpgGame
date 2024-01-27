using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using Utils.ObjectsPool;
using World.Ability;
using World.Ability.AbilitiesTypes;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerSpellCastSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _player = default;
        
        private readonly EcsPoolInject<AbilityComp> _abilityPool = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilities = default;
        private readonly EcsPoolInject<ReleasedAbilityComp> _releasedAbilityPool = default;
        
        private readonly EcsCustomInject<CursorService> _cs = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<PoolService> _ps = default;
        
        private readonly EcsWorldInject _world = default;
        
        [EcsUguiNamed(Idents.UI.PlayerAbilityView)]
        private readonly GameObject _abilityView = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEntity in _player.Value)
            {
                ref var input = ref _player.Pools.Inc2.Get(playerEntity);
                ref var rpg = ref _player.Pools.Inc3.Get(playerEntity);

                if (input.SkillList)
                {
                    _abilityView.SetActive(!_abilityView.activeSelf);
                }
                
                if (rpg.IsDead || input.FreeCursor || _cs.Value.CursorVisible) return;

                if (input.Skill1)
                {
                    if (_sd.Value.fastSkillViews[0].AbilityIdx.Unpack(_world.Value, out var unpackedSkill))
                        TryUseAbility(unpackedSkill, playerEntity);
                }

                if (input.Skill2)
                {
                    if (_sd.Value.fastSkillViews[1].AbilityIdx.Unpack(_world.Value, out var unpackedSkill))
                        TryUseAbility(unpackedSkill, playerEntity);
                }

                if (input.Skill3)
                {
                    if (_sd.Value.fastSkillViews[2].AbilityIdx.Unpack(_world.Value, out var unpackedSkill))
                        TryUseAbility(unpackedSkill, playerEntity);
                }
            }
        }

        private void TryUseAbility(int skillIdx, int entity)
        {
            ref var hasAbilities = ref _hasAbilities.Value.Get(entity);
            ref var rpg = ref _player.Pools.Inc3.Get(entity);

            foreach (var abilityPacked in hasAbilities.Entities)
            {
                if (abilityPacked.Unpack(_world.Value, out var unpackedEntity))
                {
                    ref var ability = ref _abilityPool.Value.Get(unpackedEntity);
                    if (unpackedEntity == skillIdx)
                    {
                        if (ability.currentDelay <= 0 && rpg.CastDelay <= 0)
                        {
                            if (rpg.Mana >= ability.costPoint)
                            {
                                rpg.Mana -= ability.costPoint;
                                ability.currentDelay = ability.abilityDelay;
                                rpg.CastDelay = _cf.Value.abilityConfiguration.totalAbilityDelay;
                                
                                var abilityObject = _ps.Value.SpellPool.Get();
                                var abilityEntity = _world.Value.NewEntity();
                                ref var releasedAbility = ref _releasedAbilityPool.Value.Add(abilityEntity);

                                releasedAbility.abilityObject = abilityObject;
                                releasedAbility.spellOwner = entity;
                                
                                foreach (var delayAbility in _sd.Value.uiSceneData.delayAbilityViews)
                                {
                                    if (delayAbility.delayImage.fillAmount <= 0)
                                    {
                                        delayAbility.delayImage.fillAmount = 1;
                                    }
                                }
                                
                                switch (ability.abilityType)
                                {
                                    // Balls
                                    case BallAbility type:
                                        abilityObject.SetWorld(_world.Value, entity, abilityEntity, _sd.Value, _ts.Value, _ps.Value);
                                        abilityObject.Cast(ability, entity);
                                        break;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}