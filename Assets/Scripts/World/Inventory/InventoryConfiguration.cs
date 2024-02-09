using System.Collections.Generic;
using UnityEngine;

namespace World.Inventory
{
    [CreateAssetMenu(fileName = "InventoryConfiguration", menuName = "World Configurations/Inventory Configuration")]
    public class InventoryConfiguration : ScriptableObject
    {
        [Header("Inventory settings")]
        [Tooltip("Inventory weight")]
        public float inventoryWeight = 100;
        
        public List<ItemData> items;
        public List<ItemData> allItems;
    }
}