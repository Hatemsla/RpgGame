using UnityEngine;
using World.Inventory.ItemTypesData;

namespace World.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Inventory Data/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Item")]
        [Tooltip("Item name")]
        public string itemName;
        [Tooltip("Item description")]
        public string itemDescription;
        [Tooltip("Item count")]
        public int itemCount;
        [Tooltip("Item weight")]
        public float itemWeight;
        [Tooltip("Item type data")]
        public ItemTypeData itemTypeData;
        [Tooltip("Item sprite")]
        public Sprite itemSprite;
        [Tooltip("Item view prefab")]
        public ItemView itemViewPrefab;
        [Tooltip("Item object prefab")]
        public ItemObject itemObjectPrefab;
        [Tooltip("Item cost")]
        public float cost;
    }
}