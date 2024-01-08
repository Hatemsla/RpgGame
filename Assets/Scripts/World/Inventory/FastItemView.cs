using System.Collections.Generic;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace World.Inventory
{
    public class FastItemView : MonoBehaviour, IPointerClickHandler
    {
        public EcsPackedEntity ItemIdx;
        public InputActionReference actionReference;
        public ItemObject itemObject;
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text itemCount;
        public TMP_Text itemBinding;

        private void OnValidate()
        {
            itemBinding.text = actionReference.action.GetBindingDisplayString(0);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ItemIdx = default;
                itemObject = null;
                itemImage.sprite = null;
                itemName.text = "";
                itemCount.text = "";
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                //TODO Заюзать предмет
            }
        }
    }
}