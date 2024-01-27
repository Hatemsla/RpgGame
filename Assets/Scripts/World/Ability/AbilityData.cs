using UnityEngine;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesObjects;
using World.Ability.AbilitiesPostEffects;
using World.Ability.AbilitiesPostEffects.AbilityPostEffectData;
using World.Ability.UI;

namespace World.Ability
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data")]
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
        public PostEffectTypeData postEffectTypeData;
        [Tooltip("Ability delay")]
        public float abilityDelay;
    }
}