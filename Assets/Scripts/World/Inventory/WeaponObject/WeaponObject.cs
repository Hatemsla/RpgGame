namespace World.Inventory.WeaponObject
{
    public abstract class WeaponObject : ItemObject, IAttackWeapon
    {
        public float damage;
        public float wasteStamina;
        
        public abstract void Attack();
    }
}