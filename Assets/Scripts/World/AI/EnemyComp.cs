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
        public float ChaseDistance;
        public float ChaseTime;
        public float UnChaseTime;
        public float CurrentChaseTime;
        public float CurrentUnChaseTime;

        public float WalkSpeed;
        public float RunSpeed;
        public float AngularWalkSpeed;
        public float AngularRunSpeed;

        public float MinDistanceToPlayer;

        public EnemyState EnemyState;
    }
}