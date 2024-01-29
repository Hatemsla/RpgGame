﻿using UnityEngine;
using World.RPG.UI;
using World.UI.PopupText;

namespace World.Configurations
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "World Configurations/UI Configuration", order = 2)]
    public class UiConfiguration : ScriptableObject
    {
        [Tooltip("How fast changes hsm bars")]
        public float hsmBarsChangeRate = 1f;
        
        [Tooltip("Popup damage text prefab")]
        public PopupDamageText popupDamageTextPrefab;
        
        [Tooltip("Popup damage life time")]
        public float popupDamageLifeTime = 1f;

        [Tooltip("Look on object layer mask")] 
        public LayerMask lookObjectMask;
    }
}