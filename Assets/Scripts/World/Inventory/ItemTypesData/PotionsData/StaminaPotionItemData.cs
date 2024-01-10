using UnityEngine;

namespace World.Inventory.ItemTypesData.PotionsData
{
    [CreateAssetMenu(fileName = "HealthPotionItemData", menuName = "Data/Inventory Data/Potion/Stamina")]
    public class StaminaPotionItemData : PotionItemData
    {
        public float staminaPercent;
    }
}