using UnityEngine;
using World.Ability;

namespace World
{
    [CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data", order = 4)]
    public class AbilityData : ScriptableObject
    {
        [Header("Ability")]
        [Tooltip("Ability's name")]
        public new string name;
        [Tooltip("Ability's cost")]
        public float costPoint;
        [Tooltip("Ability's damage")]
        public float damage;
        [Tooltip("Ability's radius")]
        public float radius;
        [Tooltip("Ability's life time")]
        public float lifeTime;
        [Tooltip("Ability's speed")]
        public float speed;
        [Tooltip("Ability's object")]
        public SpellObject spell;
    }
}