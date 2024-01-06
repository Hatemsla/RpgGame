using UnityEngine;
using World.Ability;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesObjects;
using World.Ability.AbilitiesPostEffects;

namespace World
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data", order = 4)]
    public class AbilityData : ScriptableObject
    {
        [Header("Ability")]
        [Tooltip("Ability name")]
        public string abilityName;
        [Tooltip("Ability description")]
        public string abilityDescription;
        [Tooltip("Ability mana cost")]
        public float costPoint;
        [Tooltip("Ability type data")] 
        public AbilityTypeData abilityTypeData;
        [Tooltip("Ability sprite")]
        public Sprite abilitySprite;
        [Tooltip("Ability view prefab")] 
        public AbilityView abilityViewPrefab;
        [Tooltip("Ability object prefab")]
        public AbilityObject abilityObjectPrefab;
        [Tooltip("Abilities post effect data")]
        public PostEffectData postEffectData;
        [Tooltip("Ability delay")]
        public float abilityDelay;
    }
}