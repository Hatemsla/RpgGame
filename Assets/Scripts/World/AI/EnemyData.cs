using System.Collections.Generic;
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
        
        [Header("Rpg settings")]
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

        [Tooltip("Chase distance for player")] 
        public float chaseDistance;
        [Tooltip("Chase time")]
        public float chaseTime;
        [Tooltip("Un Chase time")]
        public float unChaseTime;

        [Tooltip("Attack delay")] 
        public float attackDelay;

        [Tooltip("Respawn delay")] 
        public float respawnDelay;
        
        [Tooltip("Start level")]
        public int startLevel = 1;
        [Tooltip("Start experience")]
        public int startExperience;
        [Tooltip("Experience needed to reach next level")]
        public List<int> experienceToNextLevel;
    }
}