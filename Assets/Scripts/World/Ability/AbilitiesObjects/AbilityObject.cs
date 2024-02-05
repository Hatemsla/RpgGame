using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ObjectsPool;
using UnityEngine;
using Utils.ObjectsPool;
using World.Ability.StatusEffects;
using World.AI;
using World.Configurations;
using World.Player;

namespace World.Ability.AbilitiesObjects
{
    public abstract class AbilityObject : MonoBehaviour, ICastAbility
    {
        public EcsPackedEntity AbilityIdx;
        protected EcsWorld World;
        protected SceneData Sd;
        protected TimeService Ts;
        protected PoolService Ps;
        protected Configuration Cf;
        protected int PlayerEntity;
        protected int SkillIdx;

        public void SetWorld(EcsWorld world, int playerEntity, int abilityEntity, int skillIdx, 
            SceneData sd, TimeService ts,PoolService ps, Configuration cf)
        {
            World = world;
            PlayerEntity = playerEntity;
            AbilityIdx = world.PackEntity(abilityEntity);
            SkillIdx = skillIdx;
            
            Sd = sd;
            Ts = ts;
            Ps = ps;
            Cf = cf;
        }

        public abstract void Cast(AbilityComp comp, int entity);
    }
}