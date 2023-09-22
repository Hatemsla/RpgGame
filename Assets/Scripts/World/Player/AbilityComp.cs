using UnityEngine;

namespace World.Player
{
    [CreateAssetMenu]
    public class AbilityComp : ScriptableObject
    {
        public string Name;
        public float CostPoint;
        public float Damage;
        public float Distance;
    }
}