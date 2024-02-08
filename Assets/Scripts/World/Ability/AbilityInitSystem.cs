using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesTypes;
using World.Ability.StatusEffects;
using World.Ability.StatusEffects.AbilityStatusEffectComp;
using World.Ability.StatusEffects.AbilityStatusEffectData;
using World.Configurations;
using World.Inventory;
using World.Player;
using World.RPG.UI;

namespace World.Ability
{
    public sealed class AbilityInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;

        private readonly EcsPoolInject<HasStatusEffect> _hasStatusEffectPool = default;
        private readonly EcsPoolInject<StatusEffectComp> _statusEffectPool = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        private readonly EcsPoolInject<AbilityComp> _abilitiesPool = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        private readonly EcsWorldInject _world = default;

        [EcsUguiNamed(Idents.UI.PlayerAbilityView)]
        private readonly RectTransform _playerAbilityView = default;

        [EcsUguiNamed(Idents.UI.FastSkillsView)]
        private readonly RectTransform _fastSkillView = default;
        
        [EcsUguiNamed(Idents.UI.CrosshairView)]
        private readonly RectTransform _crosshairView = default;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);
                ref var hasAbilities = ref _hasAbilitiesPool.Value.Add(entity);
                ref var hasStatusEffect = ref _hasStatusEffectPool.Value.Add(entity);
                var abilities = _cf.Value.abilityConfiguration.abilityDatas;
                
                _playerAbilityView.gameObject.SetActive(false);
                
                var playerAbilityViewContent = _playerAbilityView.GetComponentInChildren<ContentView>();
                playerAbilityViewContent.currentEntity = entity;
                
                for (var i = 0; i < abilities.Count; i++)
                {
                    var abilityData = abilities[i];
                    var abilityEntity = _world.Value.NewEntity();
                    var abilityPackedEntity = _world.Value.PackEntity(abilityEntity);
                    ref var abilityComp = ref _abilitiesPool.Value.Add(abilityEntity);
                    
                    abilityComp.Name = abilityData.abilityName;
                    abilityComp.Description = abilityData.abilityDescription;
                    abilityComp.CostPoint = abilityData.costPoint;
                    abilityComp.OwnerEntity = entity;
                    abilityComp.AbilityDelay = abilityData.abilityDelay;
                    abilityComp.AbilityType = DefineAbilityType(abilityData.abilityTypeData);
                    abilityComp.StatusEffect = DefineStatusEffectComp(abilityData.statusEffect, entity, hasStatusEffect);

                    var abilityView = Object.Instantiate(abilityData.abilityViewPrefab, Vector3.zero,
                        Quaternion.identity);
                    abilityView.transform.SetParent(playerAbilityViewContent.transform);
                    abilityComp.AbilityView = abilityView;
                    
                    abilityComp.AbilityView.abilityImage.sprite = abilityData.abilityViewPrefab.abilityImage.sprite;
                    abilityComp.AbilityView.AbilityIdx = abilityPackedEntity;
                    abilityComp.AbilityView.AbilityName = abilityData.abilityName;
                    abilityComp.AbilityView.AbilityDescription = abilityData.abilityDescription;
                    abilityComp.AbilityView.AbilityParams = abilityData.costPoint.ToString();
                    abilityComp.AbilityView.SetWorld(_world.Value, entity, _sd.Value);
                    
                    abilityComp.AbilityView.SetViews(_playerAbilityView, _fastSkillView, _crosshairView);

                    var abilityObject = Object.Instantiate(abilityData.abilityObjectPrefab,
                        playerComp.Transform.position + playerComp.Transform.forward,
                        abilityData.abilityObjectPrefab.transform.rotation);
                    abilityObject.transform.SetParent(playerComp.Transform);
                    abilityObject.gameObject.SetActive(false);

                        _sd.Value.fastSkillViews[i].AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].abilityImage.sprite = abilityData.abilitySprite;
                        _sd.Value.fastSkillViews[i].abilityName.text = abilityData.abilityName;
                        _sd.Value.fastSkillViews[i].GetComponentInChildren<DelayAbilityView>().AbilityIdx =
                            abilityPackedEntity;

                    hasAbilities.Entities.Add(abilityPackedEntity);
                }
            }

        }

        private AbilityType DefineAbilityType(AbilityTypeData abilityTypeData)
        {
            AbilityType value = null;
            switch (abilityTypeData)
            {
                // Spells
                case DirectionalAbilityData directionalData:
                    switch (directionalData)
                    {
                        case BallAbilityData data:
                            value = new BallAbility();
                            ((BallAbility)value).Damage = data.damage;
                            ((BallAbility)value).Distance = data.distance;
                            ((BallAbility)value).Speed = data.speed;
                            break;
                    }
                    break;
            }
            return value;
        }

        private StatusEffectComp DefineStatusEffectComp(StatusEffectData effectData, int entity, HasStatusEffect hasStatusEffect)
        {
            ref var playerComp = ref _playerFilter.Pools.Inc1.Get(entity);

            var statusEffectEntity = _world.Value.NewEntity();
            var statusEffectPackedEntity = _world.Value.PackEntity(statusEffectEntity);
            ref var statusEffectComp = ref _statusEffectPool.Value.Add(statusEffectEntity);

            statusEffectComp.statusEffectLifeTime = effectData.statusEffectLifeTime;
            statusEffectComp.statusEffectType = DefineStatusEffectType(effectData.statusEffectTypeData);

            var statusEffectObject = Object.Instantiate(effectData.statusEffectObjectPrefab,
                playerComp.Transform.position + playerComp.Transform.forward,
                effectData.statusEffectObjectPrefab.transform.rotation);
            statusEffectObject.transform.SetParent(playerComp.Transform);
            statusEffectObject.gameObject.SetActive(false);
            
            hasStatusEffect.Entities.Add(statusEffectPackedEntity);

            return statusEffectComp;
        }
        
        private StatusEffectType DefineStatusEffectType(StatusEffectTypeData statusEffectTypeData)
        {
            StatusEffectType value = null;
            switch (statusEffectTypeData)
            {
                // Fire Status Effect
                case FireStatusEffectData data:
                    value = new FireStatusEffect();
                    ((FireStatusEffect)value).Damage = data.damage;
                    break;
            }
            return value;
        }
    }
}