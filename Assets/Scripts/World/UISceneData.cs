using System.Collections.Generic;
using UnityEngine;
using World.Inventory.Canvases;
using World.RPG.UI;

namespace World
{
    public class UISceneData : MonoBehaviour
    {
        [Tooltip("Experience slider view")]
        public ExperienceSliderView experienceSliderView;

        [Tooltip("Main canvas")] 
        public MainCanvas mainCanvas;
        
        [Tooltip("Stats views")]
        public List<StatView> statsViews;
    }
}