using UnityEngine;

namespace World.Ability.AbilitiesObjects
{
    public abstract class DirectionalAbilityObject : AbilityObject
    {
        [HideInInspector] public float damage;
        [HideInInspector] public float direction;
        [HideInInspector] public Vector3 startDirection;
        [HideInInspector] public Vector3 endDirection;
        
        //public abstract override void Cast();
    }
}