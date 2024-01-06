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
                        ref var abil = ref _ability.Value.Add(abilityEntity);
                        
                        abil.Name = abilityData.name;
                        abil.Description = abilityData.abilityDescription;
                        abil.CostPoint = abilityData.costPoint;
                        abil.OwnerEntity = entity;
                        abil.abilityType = DefineAbilityType(abilityData.abilityTypeData);

                        var abilityView = Object.Instantiate(abilityData.abilityViewPrefab, Vector3.zero,
                            Quaternion.identity);
                        abil.abilityView.abilityImage.sprite = abilityData.abilitySprite;
                        
                        hasAbilities.Entities.Add(abilityPackedEntity);
                    }
                }
            }

        }

        public AbilityType DefineAbilityType(AbilityTypeData abilityTypeData)
        {
            AbilityType value = null;
            switch (abilityTypeData)
            {
                // Spells
                case SpellAbilityData data:
                    value = new AbilitySpell();
                    ((AbilitySpell)value).Damage = data.damage;
                    ((AbilitySpell)value).Distance = data.distance;
                    ((AbilitySpell)value).Speed = data.speed;
                    break;
            }
            return value;
        }
    }
}