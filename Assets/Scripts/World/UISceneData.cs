using System.Collections.Generic;
using UnityEngine;
using World.Inventory;
using World.Inventory.Canvases;
using World.Player.UI;
using World.RPG.UI;
using World.Trader.UI;
using World.UI;

namespace World
{
    public class UISceneData : MonoBehaviour
    {
        [Tooltip("Experience slider view")]
        public ExperienceSliderView experienceSliderView;

        [Tooltip("Main canvas")] 
        public MainCanvas mainCanvas;

        [Tooltip("Trader shop")] 
        public TraderShopView traderShopView;

        public ExitMenu exitMenu;
        
        public PlayerInventoryGoldAmount playerInventoryGoldAmount;
        public RectTransform playerInventoryView;
        public RectTransform chestInventoryView;
        public RectTransform fastItemsView;
        public RectTransform deleteFormView;
        public RectTransform crosshairView;
        public InventoryWeightView playerInventoryWeightText;
        public InventoryWeightView chestInventoryWeightText;
        
        [Tooltip("Level stats views")]
        public List<LevelStatView> levelStatsViews;
        
        [Tooltip("Stats views")]
        public List<StatView> statsViews;

        [Tooltip("Delay image")]
        public List<DelayAbilityView> delayAbilityViews;
    }
}