using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesTypes;
using World.Configurations;
using World.Inventory;
using World.Player;

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
                        
                        abilityComp.name = abilityData.abilityName;
                        abilityComp.description = abilityData.abilityDescription;
                        abilityComp.costPoint = abilityData.costPoint;
                        abilityComp.ownerEntity = entity;
                        abilityComp.abilityDelay = abilityData.abilityDelay;
                        abilityComp.abilityType = DefineAbilityType(abilityData.abilityTypeData);

                        var abilityView = Object.Instantiate(abilityData.abilityViewPrefab, Vector3.zero,
                            Quaternion.identity);
                        abilityView.transform.SetParent(playerAbilityViewContent.transform);
                        abilityComp.abilityView = abilityView;
                        
                        abilityComp.abilityView.abilityImage.sprite = abilityData.abilityViewPrefab.abilityImage.sprite;
                        abilityComp.abilityView.AbilityIdx = abilityPackedEntity;
                        abilityComp.abilityView.AbilityName = abilityData.abilityName;
                        abilityComp.abilityView.AbilityDescription = abilityData.abilityDescription;
                        abilityComp.abilityView.AbilityParams = abilityData.costPoint.ToString();
                        abilityComp.abilityView.SetWorld(_world.Value, entity, _sd.Value);
                        
                        abilityComp.abilityView.SetViews(_playerAbilityView, _fastSkillView, _crosshairView);

                        var abilityObject = Object.Instantiate(abilityData.abilityObjectPrefab,
                            playerComp.Transform.position + playerComp.Transform.forward,
                            abilityData.abilityObjectPrefab.transform.rotation);
                        abilityObject.transform.SetParent(playerComp.Transform);
                        abilityObject.gameObject.SetActive(false);

                        abilityComp.abilityView.abilityObject = abilityObject;
                        abilityComp.abilityView.abilityObject.AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].abilityObject = abilityObject;
                        _sd.Value.fastSkillViews[i].abilityObject.AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].AbilityIdx = abilityPackedEntity;
                        _sd.Value.fastSkillViews[i].abilityImage.sprite = abilityData.abilitySprite;
                        _sd.Value.fastSkillViews[i].abilityName.text = abilityData.abilityName;

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
    }
}