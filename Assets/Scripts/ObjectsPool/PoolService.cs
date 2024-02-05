using Utils.ObjectsPool;
using World.Ability.AbilitiesObjects;
using World.Ability.StatusEffects.StatusEffectObjects;
using World.AI;
using World.UI.PopupText;

namespace ObjectsPool
{
    public sealed class PoolService
    {
        public PoolBase<AbilityObject> SpellPool;
        public PoolBase<StatusEffectObject> StatusEffectPool;
        public PoolBase<EnemyView> EnemyPool;
        public PoolBase<PopupDamageText> PopupDamageTextPool;
    }
}