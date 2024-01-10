using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Utils.ObjectsPool;
using World.Ability;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesObjects;
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
        private readonly EcsPoolInject<ReleasedAbilityComp> _spell = default;
        
        private readonly EcsCustomInject<CursorService> _cs = default;
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
                        if (rpg.Mana >= ability.costPoint)
                        {
                            rpg.Mana -= ability.costPoint;
                            Debug.Log(ability.abilityType);
                            switch (ability.abilityType)
                            {
                                // Balls
                                case BallAbility type:
                                    Debug.Log("Каст");
                                    InitializeBallAbility(ability, entity);
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

        private void InitializeBallAbility(AbilityComp ability, int entity)
        {
            var player = _player.Pools.Inc1.Get(entity);

            var centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            var ray = _sd.Value.mainCamera.OutputCamera.ScreenPointToRay(centerOfScreen);
            Vector3 abilityDirection;

            if (Physics.Raycast(ray, out var hitInfo, ((BallAbility)ability.abilityType).Distance))
                abilityDirection = hitInfo.point;
            else
                abilityDirection = ray.GetPoint(((BallAbility)ability.abilityType).Distance);

            var journeyLenght = Vector3.Distance(player.Transform.position + player.Transform.forward,
                abilityDirection);
            var startTime = _ts.Value.Time;

            //          //          //          //          //
            
            var abilityObject = _ps.Value.SpellPool.Get();
            abilityObject.transform.position = player.Transform.position + player.Transform.forward;

            var abilityEntity = _world.Value.NewEntity();
            var abilityPackedEntity = _world.Value.PackEntity(abilityEntity);
            ref var releasedAbility = ref _spell.Value.Add(abilityEntity);

            releasedAbility.abilityObject = abilityObject;
            releasedAbility.spellOwner = entity;

            abilityObject.PoolService = _ps.Value;
            ((BallAbilityObject)abilityObject).TimeService = _ts.Value;

            ((BallAbilityObject)abilityObject).damage = ((BallAbility)ability.abilityType).Damage;
            ((BallAbilityObject)abilityObject).startTime = startTime;
            ((BallAbilityObject)abilityObject).startDirection = player.Transform.position + player.Transform.forward;  
            ((BallAbilityObject)abilityObject).direction = journeyLenght;
            ((BallAbilityObject)abilityObject).endDirection = abilityDirection;  
            ((BallAbilityObject)abilityObject).speed = ((BallAbility)ability.abilityType).Speed;

            abilityObject.AbilityIdx = abilityPackedEntity;
            abilityObject.SetWorld(_world.Value, entity);
        }
    }
}