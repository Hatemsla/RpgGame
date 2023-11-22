using UnityEngine;
using World.Ability;
using World.AI;
using World.Inventory;

namespace Utils.ObjectsPool
{
    public sealed class PoolService
    {
        public PoolBase<SpellObject> SpellPool;
        public PoolBase<EnemyView> EnemyPool;
    }
}