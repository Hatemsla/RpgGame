using UnityEngine;
using World.Ability;
using World.Ability.AbilitiesObjects;
using World.AI;
using World.Inventory;

namespace Utils.ObjectsPool
{
    public sealed class PoolService
    {
        public PoolBase<AbilityObject> SpellPool;
        public PoolBase<EnemyView> EnemyPool;
    }
}