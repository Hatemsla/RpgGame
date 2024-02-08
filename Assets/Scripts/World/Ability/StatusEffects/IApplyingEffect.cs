using World.AI;
using World.RPG;

namespace World.Ability.StatusEffects
{
    public interface IApplyingEffect
    {
        void Applying(EnemyView enemyView, StatusEffectComp effect);
    }
}