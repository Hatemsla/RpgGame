using Fusion;
using UnityEngine;

namespace World.Network
{
    [CreateAssetMenu(fileName = "NetworkConfiguration", menuName = "World Configurations/Network Configuration")]
    public class NetworkConfiguration : ScriptableObject
    {
        public NetworkRunner networkRunnerPrefab;
    }
}