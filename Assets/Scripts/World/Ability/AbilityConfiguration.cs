using System.Collections.Generic;
using UnityEngine;

namespace World
{
    [CreateAssetMenu(fileName = "AbilityConfiguration", menuName = "Data/Ability Configuration", order = 3)]
    public class AbilityConfiguration : ScriptableObject
    {
        public List<AbilityData> AbilityDatas;
    }
}