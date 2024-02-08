using UnityEngine;
using World.Inventory;
using World.Inventory.ItemTypes;
using World.Inventory.ItemTypes.Potions;
using World.Inventory.ItemTypes.Weapons;
using World.Inventory.ItemTypesData;
using World.Inventory.ItemTypesData.PotionsData;
using World.Inventory.ItemTypesData.WeaponsData;

namespace Utils
{
    public static class Utils
    {
        public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + (newMax - newMin) * ((value - oldMin) / (oldMax - oldMin));
        }
        
        public static bool IsInScene(GameObject obj)
        {
            return obj.scene.name != null && !obj.scene.name.Equals("");
        }
        
        public static void ResetFastItemView(FastItemView ft)
        {
            ft.ItemIdx = default;
            ft.itemObject = null;
            ft.itemImage.sprite = null;
            ft.itemName.text = "";
            ft.itemCount.text = "";
        }
        
        public static ItemType DefineItemType(ItemTypeData itemTypeData)
        {
            ItemType value = null;
            switch (itemTypeData)
            {
                // Potions
                case HealthPotionItemData data:
                    value = new ItemHealthPotion();
                    ((ItemHealthPotion)value).HealthPercent = data.healthPercent;
                    break;
                case ManaPotionItemData data:
                    value = new ItemManaPotion();
                    ((ItemManaPotion)value).ManaPercent = data.manaPercent;
                    break;
                case StaminaPotionItemData data:
                    value = new ItemStaminaPotion();
                    ((ItemStaminaPotion)value).StaminaPercent = data.staminaPercent;
                    break;
                // Weapons
                case SwordWeaponItemData data:
                    value = new ItemSwordWeapon();
                    ((ItemSwordWeapon)value).Damage = data.damage;
                    ((ItemSwordWeapon)value).WasteStamina = data.wasteStamina;
                    break;
                case ShieldWeaponItemData data:
                    value = new ItemShieldWeapon();
                    ((ItemShieldWeapon)value).DamageAbsorption = data.damageAbsorption;
                    break;
                case BowWeaponItemData data:
                    value = new ItemBowWeapon();
                    ((ItemBowWeapon)value).Damage = data.damage;
                    ((ItemBowWeapon)value).Distance = data.distance;
                    break;
                // Tools
                case ToolItemData data:
                    value = new ItemTool();
                    ((ItemTool)value).Durability = data.durability;
                    break;
            }

            return value;
        }
    }
}