using UnityEngine;
using World.Inventory.ItemTypesData;

namespace World.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Inventory Data/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string itemDescription;
        public int itemCount;
        public float itemWeight;
        public ItemTypeData itemTypeData;
        public Sprite itemSprite;
        public ItemView itemViewPrefab;
        public ItemObject itemObjectPrefab;
        public float cost;
    }
}