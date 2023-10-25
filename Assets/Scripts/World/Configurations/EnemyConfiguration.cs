﻿using System.Collections.Generic;
using UnityEngine;
using World.AI;

namespace World.Configurations
{
    [CreateAssetMenu(fileName = "EnemyConfiguration", menuName = "World Configurations/Enemy Configuration")]
    public class EnemyConfiguration : ScriptableObject
    {
        [Header("Enemy settings")] 
        [Tooltip("Enemy prefab")]
        public EnemyView enemyPrefab;

        [Tooltip("Enemies data")] 
        public List<EnemyData> enemiesData;
    }
}