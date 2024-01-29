using UnityEngine;
using World.Ability.StatusEffects.AbilityStatusEffectData;
using World.Ability.StatusEffects.StatusEffectObjects;

namespace World.Ability.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectData", menuName = "Data/Ability Data/Status Effect Data")]
    public class StatusEffectData : ScriptableObject
    {
        [Header("Status Effect")] 
        [Tooltip("Status Effect object prefab")]
        public StatusEffectObject statusEffectObject1Prefab;
        [Tooltip("Status Effect type data")] 
        public StatusEffectTypeData statusEffectTypeData;
    }
}