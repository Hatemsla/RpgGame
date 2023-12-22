using UnityEngine;

namespace World.Inventory.ItemTypesData.PotionsData
{
    [CreateAssetMenu(fileName = "HealthPotionItemData", menuName = "Data/Inventory Data/Potion/Health")]
    public class HealthPotionItemData : PotionItemData
    {
        public int healthPercent;
    }
}