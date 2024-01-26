using World.Ability.AbilitiesTypes;
using World.Ability.UI;

namespace World.Ability
{
    public struct AbilityComp
    {
        public string name;
        public string description;
        public float costPoint;
        public float ownerEntity;
        public float abilityDelay;
        public float currentDelay;
        public AbilityView abilityView;
        public AbilityType abilityType;
    }
}