using UnityEngine;

namespace World.Inventory.ItemTypesData.WeaponsData
{
    [CreateAssetMenu(fileName = "BowWeaponItemData", menuName = "Data/Inventory Data/Weapon/Bow")]
    public class BowWeaponItemData : WeaponItemData
    {
        public float damage;
        public float distance;
    }
}