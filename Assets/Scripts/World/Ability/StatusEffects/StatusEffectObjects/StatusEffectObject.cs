using Leopotam.EcsLite;
using ObjectsPool;
using UnityEngine;
using Utils.ObjectsPool;
using World.AI;
using World.Configurations;
using World.RPG;

namespace World.Ability.StatusEffects.StatusEffectObjects
{
    public abstract class StatusEffectObject : MonoBehaviour, IApplyingEffect
    {
        public EcsPackedEntity EffectIdx;
        private protected EcsWorld World;
        private protected SceneData Sd;
        private protected TimeService Ts;
        private protected PoolService Ps;
        private protected Configuration Cf;
        private protected int PlayerEntity;

        public void SetWorld(EcsWorld world, int entity, int statusEffectEntity, SceneData sd,
            TimeService ts, PoolService ps, Configuration cf)
        {
            World = world;
            PlayerEntity = entity;
            EffectIdx = world.PackEntity(statusEffectEntity);
            
            Sd = sd;
            Ts = ts;
            Ps = ps;
            Cf = cf;
        }

        public abstract void Applying(EnemyView enemyView, StatusEffectComp effect);
    }
}