using Leopotam.EcsLite;
using UnityEngine;
using Utils.ObjectsPool;

namespace World.Ability.AbilitiesObjects
{
    public abstract class DirectionalAbilityObject : AbilityObject
    {
        [HideInInspector] public float damage;
        [HideInInspector] public float direction;
        [HideInInspector] public Vector3 startDirection;
        [HideInInspector] public Vector3 endDirection;
        
        public abstract override void Cast(AbilityComp comp, int entity);
    }
}