using UnityEngine;

namespace World.Inventory.ItemTypesData.PotionsData
{
    [CreateAssetMenu(fileName = "ManaPotionItemData", menuName = "Data/Inventory Data/Potion/Mana")]
    public class ManaPotionItemData : PotionItemData
    {
        public float manaPercent;
    }
}