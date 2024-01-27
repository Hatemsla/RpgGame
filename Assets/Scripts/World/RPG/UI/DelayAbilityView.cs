using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.RPG.UI
{
    public class DelayAbilityView : MonoBehaviour
    {
        public EcsPackedEntity AbilityIdx;
        public Image delayImage;
        public TMP_Text delayTimer;
    }
}