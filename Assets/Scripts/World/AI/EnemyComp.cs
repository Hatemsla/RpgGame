using UnityEngine;
using UnityEngine.AI;
using World.AI.Navigation;

namespace World.AI
{
    public struct EnemyComp
    {
        public int EnemyIndex;
        public string EnemyName;
        public Transform Transform;
        public EnemyView EnemyView;
        public int TargetIndex;
        public NavMeshAgent Agent;
        
        public float MinDamage;
        public float MaxDamage;
        public float AttackDelay;
        
        public EnemyState EnemyState;
    }
}