using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public EcsPackedEntity ItemIdx;
        public ItemObject itemObject;
        public Image itemImage;
        public RectTransform playerInventoryView;
        public RectTransform chestInventoryView;
        public TMP_Text inventoryWeightText;
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
                            itemObject.gameObject.SetActive(!itemObject.gameObject.activeSelf);
                        else
                            itemObject.gameObject.SetActive(false);
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
            {
                if (IsItemInPlayerInventory(transform.position, chestInventoryView))
                {
                    transform.SetParent(chestInventoryView);
                }
                else if (IsItemInPlayerInventory(transform.position, playerInventoryView))
                {
                    transform.SetParent(playerInventoryView);
                }
                else
                {
                    DestroyItem();   
                }
            }
            else
            {
                transform.SetParent(_parentAfterDrag);
            }
        }

        private void DestroyItem()
        {
            if (ItemIdx.Unpack(_world, out var unpackedEntity))
            {
                ref var item = ref _itemsPool.Get(unpackedEntity);

                ref var inventory = ref _inventoryPool.Get(_playerEntity);
                inventory.CurrentWeight -= item.Weight;
                inventoryWeightText.text = $"Вес: {inventory.CurrentWeight}/{inventory.MaxWeight}";
                
                Destroy(itemObject.gameObject);
                
                _itemsPool.Del(unpackedEntity);
            }

            Destroy(transform.gameObject);
        }

        private bool IsItemOutInventory(Vector3 position)
        {
            var minPosition = playerInventoryView.TransformPoint(playerInventoryView.rect.min);
            var maxPosition = playerInventoryView.TransformPoint(playerInventoryView.rect.max);

            return position.x < minPosition.x || position.x > maxPosition.x || position.y < minPosition.y ||
                   position.y > maxPosition.y;
        }
        
        private bool IsItemInPlayerInventory(Vector3 position, RectTransform view)
        {
            var minPosition = view.TransformPoint(view.rect.min);
            var maxPosition = view.TransformPoint(view.rect.max);

            return position.x > minPosition.x || position.x < maxPosition.x || position.y > minPosition.y ||
                   position.y < maxPosition.y;
        }
    }
}