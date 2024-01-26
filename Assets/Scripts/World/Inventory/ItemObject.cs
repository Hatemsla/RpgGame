using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;
using World.Player;

namespace World.Inventory
{
    public abstract class ItemObject : MonoBehaviour
    {
        public EcsPackedEntity ItemIdx;
        
        public EcsWorld World;
        public PlayerComp PlayerComp;
        public AnimationComp AnimationComp;
        public PoolService Ps;
        public TimeService Ts;
    }
}