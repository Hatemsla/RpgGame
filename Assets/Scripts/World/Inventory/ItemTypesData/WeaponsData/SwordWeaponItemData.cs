using UnityEngine;

namespace World.Inventory.ItemTypesData.WeaponsData
{
    [CreateAssetMenu(fileName = "SwordWeaponItemData", menuName = "Data/Inventory Data/Weapon/Sword")]
    public class SwordWeaponItemData : WeaponItemData
    {
        public float damage;
        public float wasteStamina;
    }
}