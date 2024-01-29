using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesTypes;
using World.Configurations;
using World.Inventory;
using World.Player;
using World.RPG.UI;

namespace World.Ability
{
    public sealed class AbilityInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp>> _playerFilter = default;

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
                var abilities = _cf.Value.abilityConfiguration.abilityDatas;
                
                _playerAbilityView.gameObject.SetActive(false);
                
                var playerAbilityViewContent = _playerAbilityView.GetComponentInChildren<ContentView>();
                playerAbilityViewContent.currentEntity = entity;
                
                for (var i = 0; i < abilities.Count; i++)
                {
                    var abilityData = abilities[i];
                    if (abilityData.abilityName == Idents.Abilities.FireBall)
                    {
                        var abilityEntity = _world.Value.NewEntity();
                        var abilityPackedEntity = _world.Value.PackEntity(abilityEntity);
                        ref var abilityComp = ref _abilitiesPool.Value.Add(abilityEntity);
                        
                        abilityComp.Name = abilityData.abilityName;
                        abilityComp.Description = abilityData.abilityDescription;
                        abilityComp.CostPoint = abilityData.costPoint;
                        abilityComp.OwnerEntity = entity;
                        abilityComp.AbilityDelay = abilityData.abilityDelay;
                        abilityComp.AbilityType = DefineAbilityType(abilityData.abilityTypeData);
                        //abilityComp.abilityPostEffect =
                        //DefineAbilityType(abilityData.postEffectData.postEffectTypeData);

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

                        abilityComp.AbilityView.abilityObject = abilityObject;
                        abilityComp.AbilityView.abilityObject.AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].abilityObject = abilityObject;
                        _sd.Value.fastSkillViews[i].abilityObject.AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].abilityImage.sprite = abilityData.abilitySprite;
                        _sd.Value.fastSkillViews[i].abilityName.text = abilityData.abilityName;
                        _sd.Value.fastSkillViews[i].GetComponentInChildren<DelayAbilityView>().AbilityIdx =
                            abilityPackedEntity;

                        hasAbilities.Entities.Add(abilityPackedEntity);
                    }
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

        /*private StatusEffectType1 DefinePostEffectType(StatusEffectTypeData1 statusEffectData1)
        {
            StatusEffectType1 value = null;
            switch (statusEffectData1)
            {
                // Explosions
                case FireStatusEffectData1 data:
                    value = new FireStatusEffect1();
                    ((FireStatusEffect1)value).Damage = data.damage; 
                    break;
            }
            return value;
        }*/
    }
}