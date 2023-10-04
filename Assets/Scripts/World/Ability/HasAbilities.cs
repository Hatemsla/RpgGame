using System.Collections.Generic;
using Leopotam.EcsLite;

namespace World.Ability
{
    public struct HasAbilities : IEcsAutoReset<HasAbilities>
    {
        public List<int> Entities;
        public void AutoReset(ref HasAbilities c)
        {
            c.Entities ??= new List<int>();
        }
    }
}