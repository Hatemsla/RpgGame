using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace World.Inventory
{
    [CreateAssetMenu(fileName = "InventoryConfiguration", menuName = "World Configurations/Inventory Configuration")]
    public class InventoryConfiguration : ScriptableObject
    {
        public List<ItemData> items;
    }
}