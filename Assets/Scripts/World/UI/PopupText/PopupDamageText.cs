using System;
using TMPro;
using UnityEngine;

namespace World.UI.PopupText
{
    public class PopupDamageText : MonoBehaviour
    {
        public TMP_Text damageText;
        public AnimationCurve opacityCurve;
        public AnimationCurve scaleCurve;
        public AnimationCurve heightCurve;
        
        public float currentTime;
    }
}