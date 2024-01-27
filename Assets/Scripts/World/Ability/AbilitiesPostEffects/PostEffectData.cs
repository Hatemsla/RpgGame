using UnityEngine;
using World.Ability.AbilitiesPostEffects.AbilityPostEffectComp;
using World.Ability.AbilitiesPostEffects.AbilityPostEffectData;
using World.Ability.AbilitiesPostEffects.PostEffectsObjects;

namespace World.Ability.AbilitiesPostEffects
{
    [CreateAssetMenu(fileName = "PostEffectData", menuName = "Data/Ability Data/Post Effect Data")]
    public class PostEffectData : ScriptableObject
    {
        [Header("Post Effect")] 
        [Tooltip("Post Effect object prefab")]
        public PostEffectObject postEffectObjectPrefab;
        [Tooltip("Post Effect type data")] 
        public PostEffectTypeData postEffectTypeData;
    }
}