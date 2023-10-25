using System.Collections.Generic;
using UnityEngine;

namespace World.Ability
{
    [CreateAssetMenu(fileName = "AbilityConfiguration", menuName = "World Configurations/Ability Configuration", order = 3)]
    public class AbilityConfiguration : ScriptableObject
    {
        public List<AbilityData> abilityDatas;
    }
}