using UnityEngine;
using World.Player;

namespace World
{
    [CreateAssetMenu(fileName = "MainConfiguration", menuName = "World Configurations/Main Configuration")]
    public class Configuration : ScriptableObject
    {
        public PlayerConfiguration playerConfiguration; 
        public UiConfiguration uiConfiguration;
        public AbilityConfiguration abilityConfiguration;
    }
}