using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Player;

namespace World.Ability.AbilitiesObjects
{
    public abstract class AbilityObject : MonoBehaviour
    {
        public EcsPackedEntity AbilityIdx;
        public PoolService PoolService;

        private protected bool _isCast;
        private protected EcsWorld _world;
        private protected int _playerEntity;
        private protected EcsPool<ReleasedAbilityComp> _releasedAbilityPool;

        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _playerEntity = entity;
            _releasedAbilityPool = _world.GetPool<ReleasedAbilityComp>();
        }
    }
}