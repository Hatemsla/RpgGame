using UnityEngine;

namespace World.Inventory.PotionsData
{
    [CreateAssetMenu(fileName = "HealthPotionItemData", menuName = "Data/Inventory Data/Health Potion")]
    public class HealthPotionItemData : ItemData
    {
        public int healthPercent;
    }
}