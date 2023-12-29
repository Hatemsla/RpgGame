using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace World.Ability
{
    public class FastSkillView : MonoBehaviour, IPointerClickHandler
    {
        public EcsPackedEntity AbilityIdx;
        public InputActionReference actionReference;
        public Image abilityImage;
        public TMP_Text abilityName;
        public TMP_Text abilityBinding;

        private void OnValidate()
        {
            abilityBinding.text = actionReference.action.GetBindingDisplayString(0);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                abilityImage.sprite = null;
                abilityName.text = "";
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                //TODO Заюзать спел
            }
        }
    }
}