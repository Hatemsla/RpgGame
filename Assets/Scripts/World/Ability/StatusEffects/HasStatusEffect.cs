using System.Collections.Generic;
using Leopotam.EcsLite;

namespace World.Ability.StatusEffects
{
    public struct HasStatusEffect : IEcsAutoReset<HasStatusEffect>
    {
        public List<EcsPackedEntity> Entities;

        public void AutoReset(ref HasStatusEffect c)
        {
            c.Entities ??= new List<EcsPackedEntity>();
        }
    }
}