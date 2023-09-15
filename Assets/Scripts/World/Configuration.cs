using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using World.Player;

namespace World
{
    [CreateAssetMenu(fileName = "MainConfiguration", menuName = "World Configurations/Main Configuration")]
    public class Configuration : ScriptableObject
    {
        public PlayerConfiguration playerConfiguration;
    }
}