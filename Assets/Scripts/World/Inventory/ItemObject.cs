using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.EventSystems;

namespace World.Inventory
{
    public class ItemObject : MonoBehaviour, IPointerClickHandler
    {
        public int itemIdx;

        public EcsPoolInject<HasItems> HasItems = default;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}