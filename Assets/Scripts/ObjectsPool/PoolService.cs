using World.Ability.AbilitiesObjects;
using World.AI;
using World.UI.PopupText;

namespace Utils.ObjectsPool
{
    public sealed class PoolService
    {
        public PoolBase<AbilityObject> SpellPool;
        public PoolBase<EnemyView> EnemyPool;
        public PoolBase<PopupDamageText> PopupDamageTextPool;
    }
}