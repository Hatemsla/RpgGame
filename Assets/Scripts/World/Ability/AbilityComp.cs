using World.Ability.AbilitiesTypes;

namespace World.Ability
{
    public struct AbilityComp
    {
        public string Name;
        public string Description;
        public float CostPoint;
        public float OwnerEntity;
        public AbilityView abilityView;
        public AbilityType abilityType;
    }
}