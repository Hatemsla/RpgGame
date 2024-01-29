using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Configurations;
using World.Player;

namespace World.Ability.AbilitiesObjects
{
    public abstract class AbilityObject : MonoBehaviour, ICastAbility
    {
        protected EcsPackedEntity AbilityIdx;
        protected EcsWorld World;
        protected SceneData Sd;
        protected TimeService Ts;
        protected PoolService Ps;
        protected Configuration Cf;
        protected int PlayerEntity;

        public void SetWorld(EcsWorld world, int playerEntity, int abilityEntity, SceneData sd, TimeService ts, PoolService ps, Configuration cf)
        {
            World = world;
            PlayerEntity = playerEntity;
            AbilityIdx = world.PackEntity(abilityEntity);

            Sd = sd;
            Ts = ts;
            Ps = ps;
            Cf = cf;
        }

        public abstract void Cast(AbilityComp comp, int entity);
    }
}