using UnityEngine;

namespace World.Configurations
{
    [CreateAssetMenu(fileName = "ChestConfiguration", menuName = "World Configurations/Chest Configuration")]
    public sealed class ChestConfiguration : ScriptableObject
    {
        [Header("Chest settings")]
        [Tooltip("Chest inventory weight")]
        public float chestInventoryMaxWeight = float.MaxValue;
    }
}