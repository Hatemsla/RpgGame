using UnityEngine;
using World.RPG.UI;

namespace World.Configurations
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "World Configurations/UI Configuration", order = 2)]
    public class UiConfiguration : ScriptableObject
    {
        [Tooltip("How fast changes hsm bars")]
        public float hsmBarsChangeRate = 1f;
    }
}