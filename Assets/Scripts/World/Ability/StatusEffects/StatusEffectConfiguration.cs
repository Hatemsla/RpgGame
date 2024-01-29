using System.Collections.Generic;
using UnityEngine;

namespace World.Ability.StatusEffects
{
    [CreateAssetMenu(fileName = "StatusEffectConfiguration", menuName = "World Configurations/StatusConfiguration", order = 4)]
    public class StatusEffectConfiguration : ScriptableObject
    {
        public List<StatusEffectData> statusEffectDatas;
    }
}