using UnityEngine;

namespace World.Ability.AbilitiesObjects
{
    public abstract class DirectionalAbilityObject : AbilityObject
    {
        public float damage;
        public float direction;
        public Vector3 startDirection;
        public Vector3 endDirection;
        
        public abstract override void Cast(AbilityComp comp, int entity);
    }
}