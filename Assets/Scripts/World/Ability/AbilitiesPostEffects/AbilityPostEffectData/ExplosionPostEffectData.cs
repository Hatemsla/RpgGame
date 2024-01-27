using UnityEngine;
using World.Ability.AbilitiesPostEffects.AbilityPostEffectComp;

namespace World.Ability.AbilitiesPostEffects.AbilityPostEffectData
{
    [CreateAssetMenu(fileName = "ExplosionPostEffect", menuName = "Data/Ability Data/Post Effect Data/Explosion")]
    public class ExplosionPostEffectData : PostEffectTypeData
    {
        public float damage;
    }
}