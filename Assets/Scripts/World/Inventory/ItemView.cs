using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using World.Player;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public EcsPackedEntity ItemIdx;
        public Image itemImage;
        public RectTransform inventoryView;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemCount;

        private Transform _parentAfterDrag;

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
        private EcsPool<HasItems> _hasItems;
        private EcsPool<ItemComp> _itemsPool;
        private EcsPool<InventoryComp> _inventoryPool; 

        public void SetWorld(EcsWorld world, int entity)
        {
            _world = world;
            _playerEntity = entity;
            _hasItems = _world.GetPool<HasItems>();
            _itemsPool = _world.GetPool<ItemComp>();
            _inventoryPool = _world.GetPool<InventoryComp>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                ref var hasItems = ref _hasItems.Get(_playerEntity);

                ItemIdx.Unpack(_world, out var currentEntity);

                foreach (var itemPacked in hasItems.Entities)
                    if (itemPacked.Unpack(_world, out var unpackedEntity))
                    {
                        ref var item = ref _itemsPool.Get(unpackedEntity);
                        if (currentEntity == unpackedEntity)
                            item.ItemObject.gameObject.SetActive(!item.ItemObject.gameObject.activeSelf);
                        else
                            item.ItemObject.gameObject.SetActive(false);
                    }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Mouse.current.position.value;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsItemOutInventory(transform.position))
                DestroyItem();
            else
                transform.SetParent(_parentAfterDrag);
        }

        private void DestroyItem()
        {
            ref var hasItems = ref _hasItems.Get(_playerEntity);

            for (var i = 0; i < hasItems.Entities.Count; i++)
                if (ItemIdx.Unpack(_world, out var unpackedEntity))
                {
                    ref var item = ref _itemsPool.Get(unpackedEntity);

                    Destroy(item.ItemObject.gameObject);
                    _itemsPool.Del(unpackedEntity);
                }

            Destroy(transform.gameObject);
        }

        private bool IsItemOutInventory(Vector3 position)
        {
            var minPosition = inventoryView.TransformPoint(inventoryView.rect.min);
            var maxPosition = inventoryView.TransformPoint(inventoryView.rect.max);

            return position.x < minPosition.x || position.x > maxPosition.x || position.y < minPosition.y ||
                   position.y > maxPosition.y;
        }
    }
}