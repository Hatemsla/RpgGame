using UnityEngine;

namespace World
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "World Configurations/UI Configuration", order = 2)]
    public class UiConfiguration : ScriptableObject
    {
        [Tooltip("How fast changes hsm bars")]
        public float hsmBarsChangeRate = 1f;
    }
}