using Leopotam.EcsLite;
using ObjectsPool;
using UnityEngine;
using World.AI;
using World.Configurations;

namespace World.Ability.StatusEffects.StatusEffectObjects
{
    public abstract class StatusEffectObject : MonoBehaviour, IApplyingEffect
    {
        public EcsPackedEntity EffectIdx;
        protected EcsWorld World;
        protected SceneData Sd;
        protected TimeService Ts;
        protected PoolService Ps;
        protected Configuration Cf;
        protected int PlayerEntity;

        public void SetWorld(EcsWorld world, int playerEntity, int statusEffectEntity, SceneData sd,
            TimeService ts, PoolService ps, Configuration cf)
        {
            World = world;
            PlayerEntity = playerEntity;
            EffectIdx = world.PackEntity(statusEffectEntity);
            
            Sd = sd;
            Ts = ts;
            Ps = ps;
            Cf = cf;
        }

        public abstract void Applying(EnemyView enemyView, StatusEffectComp effect);
    }
}