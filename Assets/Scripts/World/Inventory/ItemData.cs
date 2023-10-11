using UnityEngine;
using World.Inventory;

namespace World
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Inventory Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public string itemDescription;
        public int itemCount;
        public ItemType itemType;
        public Sprite itemSprite;
        public ItemView itemViewPrefab;
        public ItemObject itemObjectPrefab;
        public float cost;
    }
}