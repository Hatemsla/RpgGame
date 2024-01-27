using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;
using World.Player;
using World.RPG;

namespace World.Inventory
{
    public abstract class ItemObject : MonoBehaviour
    {
        public EcsPackedEntity ItemIdx;
        
        public EcsWorld World;
        public int playerEntity;
        public PoolService Ps;
        public TimeService Ts;
    }
}