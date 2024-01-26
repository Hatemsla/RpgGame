namespace World.Inventory.WeaponObject
{
    public abstract class WeaponObject : ItemObject, IAttackWeapon
    {
        public float damage;
        
        public abstract void Attack();
    }
}