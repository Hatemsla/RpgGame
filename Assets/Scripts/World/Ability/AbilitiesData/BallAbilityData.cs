using UnityEngine;

namespace World.Ability.AbilitiesData
{
    [CreateAssetMenu(fileName = "BallAbilityData", menuName = "Data/Ability Datas/Ball Ability")]
    public abstract class BallAbilityData : DirectionalAbilityData
    {
        public float speed;
    }
}