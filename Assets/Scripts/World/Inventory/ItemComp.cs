namespace World.Inventory
{
    public struct ItemComp
    {
        public string ItemName;
        public string ItemDescription;
        public float Cost;
        public float Weight;
        public ItemView ItemView;
        public ItemObject ItemObject;
        public bool Used;
    }
}