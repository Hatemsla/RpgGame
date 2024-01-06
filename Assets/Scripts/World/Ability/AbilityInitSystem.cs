using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
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
        private readonly EcsFilterInject<Inc<PlayerComp, InventoryComp>> _playerFilter = default;
        
        private readonly EcsWorldInject _world = default;
        
        private readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;
        private readonly EcsPoolInject<AbilityComp> _ability = default;
        
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _playerFilter.Value)
            {
                ref var hasAbilities = ref _hasAbilitiesPool.Value.Add(entity);
                foreach (var abilityData in _cf.Value.abilityConfiguration.abilityDatas)
                {
                    if (abilityData.name == Idents.Abilities.FireBall)
                    {
                        var abilityEntity = _world.Value.NewEntity();
                        var abilityPackedEntity = _world.Value.PackEntity(abilityEntity);
                        ref var abilityComp = ref _ability.Value.Add(abilityEntity);
                        
                        abilityComp.name = abilityData.name;
                        abilityComp.description = abilityData.abilityDescription;
                        abilityComp.costPoint = abilityData.costPoint;
                        abilityComp.ownerEntity = entity;
                        abilityComp.abilityType = DefineAbilityType(abilityData.abilityTypeData);

                        var abilityView = Object.Instantiate(abilityData.abilityViewPrefab, Vector3.zero,
                            Quaternion.identity);
                        abilityView.abilityObject = abilityData.abilityObjectPrefab;
                        
                        abilityComp.abilityView.abilityImage.sprite = abilityData.abilityViewPrefab.abilityImage.sprite;
                        
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
                            ((BallAbility)value).damage = data.damage;
                            ((BallAbility)value).distance = data.distance;
                            ((BallAbility)value).speed = data.speed;
                            break;
                    }
                    break;
            }
            return value;
        }
    }
}