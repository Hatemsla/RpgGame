using UnityEngine;
using World.Ability.AbilitiesData;
using World.Ability.AbilitiesObjects;
using World.Ability.StatusEffects;
using World.Ability.UI;

namespace World.Ability
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Datas/AbilityData")]
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
        [Tooltip("Status effect data")]
        public StatusEffectData statusEffect;
        /*[Tooltip("Ability result data")]
        public*/ 
        [Tooltip("Ability delay")]
        public float abilityDelay;
    }
}