using System.Collections.Generic;
using UnityEngine;

namespace World.Inventory.Chest
{
    public sealed class ChestObject : MonoBehaviour
    {
        public List<ItemData> items;

        public RectTransform chestInventoryView;
        
        private void OnTriggerEnter(Collider other)
        {
            
        }
    }
}