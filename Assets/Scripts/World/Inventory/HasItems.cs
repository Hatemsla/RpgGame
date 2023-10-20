using System.Collections.Generic;
using Leopotam.EcsLite;

namespace World.Inventory
{
    public struct HasItems : IEcsAutoReset<HasItems>
    {
        public List<EcsPackedEntity> Entities;
        
        public void AutoReset(ref HasItems c)
        {
            c.Entities ??= new List<EcsPackedEntity>(128);
        }
    }
}