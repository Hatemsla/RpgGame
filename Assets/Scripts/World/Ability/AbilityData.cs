using UnityEngine;

namespace World
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data", order = 4)]
    public class AbilityData : ScriptableObject
    {
        [Header("Ability")]
        [Tooltip("Ability's name")]
        public string name;
        [Tooltip("Ability's cost")]
        public float costPoint;
        [Tooltip("Ability's damage")]
        public float damage;
        [Tooltip("Ability's distance")]
        public float distance;
        
    }
}