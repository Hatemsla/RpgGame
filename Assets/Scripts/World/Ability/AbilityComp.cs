using World.Ability.AbilitiesTypes;

namespace World.Ability
{
    public struct AbilityComp
    {
        public string name;
        public string description;
        public float costPoint;
        public float ownerEntity;
        public AbilityView abilityView;
        public AbilityType abilityType;
    }
}