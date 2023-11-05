using UnityEngine;

namespace World.AI
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Enemy settings")] 
        [Tooltip("Enemy name")]
        public string enemyName;
        [Tooltip("Enemy prefab")] 
        public EnemyView enemyView;
        
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

        [Tooltip("Min damage")] 
        public float minDamage;
        [Tooltip("Max damage")]
        public float maxDamage;

        [Tooltip("Attack delay")] 
        public float attackDelay;
    }
}