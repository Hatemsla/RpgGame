using System.Collections.Generic;
using Leopotam.EcsLite;

namespace World.AI.Navigation
{
    public struct HasEnemies : IEcsAutoReset<HasEnemies>
    {
        public List<EcsPackedEntity> Entities;
        
        public void AutoReset(ref HasEnemies c)
        {
            c.Entities ??= new List<EcsPackedEntity>(128);
        }
    }
}