using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour, IPointerClickHandler
    {
        public int itemIdx;
        public Image itemImage;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemCount;

        public string ItemName
        {
            get => itemName.text;
            set => itemName.text = value;
        }

        public string ItemDescription
        {
            get => itemDescription.text;
            set => itemDescription.text = value;
        }

        public string ItemCount
        {
            get => itemCount.text;
            set => itemCount.text = value;
        }

        private EcsWorld _world;
        private int _playerEntity;
        public EcsPool<HasItems> _hasItems;
        public EcsPool<ItemComp> _itemsPool;

        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _playerEntity = entity;
            _hasItems = _world.GetPool<HasItems>();
            _itemsPool = _world.GetPool<ItemComp>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ref var hasItems = ref _hasItems.Get(_playerEntity);

            for (var i = 0; i < hasItems.Entities.Count; i++)
            {
                var itemEntity = hasItems.Entities[i];
                ref var item = ref _itemsPool.Get(itemEntity);

                if (i == itemIdx)
                {
                    item.ItemObject.gameObject.SetActive(!item.ItemObject.gameObject.activeSelf);
                }
                else
                {
                    item.ItemObject.gameObject.SetActive(false);
                }
            }
        }
    }
}