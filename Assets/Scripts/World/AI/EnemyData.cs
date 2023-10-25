using UnityEngine;

namespace World.AI
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Enemy settings")] 
        [Tooltip("Enemy name")]
        public string enemyName;
        
        [Header("Enemy rpg settings")]
        [Tooltip("Start enemy health points")]
        public float health;
        [Tooltip("Start enemy stamina points")]
        public float stamina;
        [Tooltip("Start enemy mana points")]
        public float mana;
        
        [Tooltip("Health recovery rate")] 
        public float healthRecovery;
        
        [Tooltip("Stamina recovery rate")]
        public float staminaRecovery;

        [Tooltip("Mana recovery rate")]
        public float manaRecovery;
    }
}