﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour, IPointerClickHandler
    {
        public int itemIdx;
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text desc;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}