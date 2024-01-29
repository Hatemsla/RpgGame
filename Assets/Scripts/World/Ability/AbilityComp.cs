using World.Ability.AbilitiesTypes;
using World.Ability.StatusEffects;
using World.Ability.UI;

namespace World.Ability
{
    public struct AbilityComp
    {
        public string Name;
        public string Description;
        public float CostPoint;
        public float OwnerEntity;
        public float AbilityDelay;
        public float CurrentDelay;
        public AbilityView AbilityView;
        public AbilityType AbilityType;
        public StatusEffectComp StatusEffect;
    }
}