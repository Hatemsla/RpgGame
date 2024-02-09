using World.Ability.AbilitiesObjects;
using World.Ability.StatusEffects.StatusEffectObjects;
using World.AI;
using World.UI.PopupText;

namespace ObjectsPool
{
    public sealed class PoolService
    {
        public PoolBase<AbilityObject> FireBallSpellPool;
        public PoolBase<AbilityObject> IcePickeSpellPool;
        public PoolBase<AbilityObject> EarthBallSpellPool;
        public PoolBase<StatusEffectObject> EarthEffectPool;
        public PoolBase<StatusEffectObject> FireStatusEffectPool;
        public PoolBase<StatusEffectObject> IceStatusEffectPool;
        public PoolBase<EnemyView> EnemyPool;
        public PoolBase<PopupDamageText> PopupDamageTextPool;
    }
}