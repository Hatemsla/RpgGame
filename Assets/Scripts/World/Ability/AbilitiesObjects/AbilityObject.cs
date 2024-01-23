using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Player;

namespace World.Ability.AbilitiesObjects
{
    public abstract class AbilityObject : MonoBehaviour, ICastAbility
    {
        public EcsPackedEntity AbilityIdx;

        private protected EcsWorld _world;
        private protected EcsPool<PlayerComp> _player;
        private protected EcsPool<ReleasedAbilityComp> _releasedAbilityPool;

        private protected SceneData _sd;
        private protected TimeService _ts;
        private protected PoolService _ps;

        private protected int _playerEntity;

        public void SetWorld(EcsWorld world, int entity, SceneData sd, TimeService ts, PoolService ps)
        {
            _world = world;
            _playerEntity = entity;
            _player = _world.GetPool<PlayerComp>();
            _releasedAbilityPool = _world.GetPool<ReleasedAbilityComp>();

            _sd = sd;
            _ts = ts;
            _ps = ps;
        }

        public abstract void Cast(AbilityComp comp, int entity);
    }
}