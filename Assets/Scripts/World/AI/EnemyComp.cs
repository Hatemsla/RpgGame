using UnityEngine;
using UnityEngine.AI;

namespace World.AI
{
    public struct EnemyComp
    {
        public Transform Transform;
        public int TargetIndex;
        public Vector3 Position;
        public Quaternion Rotation;
        public NavMeshAgent Agent;
    }
}