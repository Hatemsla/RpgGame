﻿using World.Inventory.ItemTypes;

namespace World.Inventory
{
    public struct ItemComp
    {
        public string ItemName;
        public string ItemDescription;
        public int Cost;
        public float Weight;
        public ItemView ItemView;
        public ItemType ItemType;
    }
}