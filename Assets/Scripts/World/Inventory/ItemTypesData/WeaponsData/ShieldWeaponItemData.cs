using UnityEngine;

namespace World.Inventory.ItemTypesData.WeaponsData
{
    [CreateAssetMenu(fileName = "ShieldWeaponItemData", menuName = "Data/Inventory Data/Weapon/Shield")]
    public class ShieldWeaponItemData : WeaponItemData
    {
        public float damageAbsorption;
    }
}