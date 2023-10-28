using UnityEngine;
using UnityEngine.AI;
using World.AI.Navigation;

namespace World.AI
{
    public struct EnemyComp
    {
        public Transform Transform;
        public int TargetIndex;
        public NavMeshAgent Agent;

        public EnemyState EnemyState;
    }
}