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
        
        [Tooltip("Level stats views")]
        public List<LevelStatView> levelStatsViews;
        
        [Tooltip("Stats views")]
        public List<StatView> statsViews;

        [Tooltip("Delay image")]
        public List<DelayAbilityView> delayAbilityViews;
    }
}