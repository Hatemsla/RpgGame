using UnityEngine;

namespace World.Ability.AbilitiesObjects
{
    public class DirectionalAbilityObject : AbilityObject
    {
        [HideInInspector] public float damage;
        [HideInInspector] public float direction;
        [HideInInspector] public Vector3 startDirection;
        [HideInInspector] public Vector3 endDirection;
    }
}