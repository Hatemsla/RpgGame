using System.Collections.Generic;
using UnityEngine;
using World.RPG.UI;

namespace World
{
    public class UISceneData : MonoBehaviour
    {
        [Tooltip("Experience slider view")]
        public ExperienceSliderView experienceSliderView;
        
        [Tooltip("Stats views")]
        public List<StatView> statsViews;
    }
}