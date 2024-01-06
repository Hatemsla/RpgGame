using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using World.Inventory.Chest;
using World.Player;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public EcsPackedEntity ItemIdx;
        public ItemObject itemObject;
        public Image itemImage;
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemDescription;
        [SerializeField] private TMP_Text itemCount;

        private Transform _parentBeforeDrag;
        private SceneData _sd;
        private bool _isDeleteEvent;

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
        private int _ownerEntity;
        private EcsPool<HasItems> _hasItems;
        private EcsPool<ItemComp> _itemsPool;
        private EcsPool<InventoryComp> _inventoryPool;
        private EcsPool<ChestComp> _chestPool;
        private EcsPool<PlayerComp> _playerPool;
        private EcsPool<DeleteEvent> _deletePool;
        private EcsFilter _deleteFilter;

        private ContentView _playerInventoryViewContent;
        private ContentView _chestInventoryViewContent;
        private DeleteFormView _deleteFormView;

        private RectTransform _crosshairView;
        private RectTransform _playerInventoryView;
        private RectTransform _chestInventoryView;
        private RectTransform _fastItemsView;

        private float _lastClickTime;
        private readonly float _doubleClickThreshold = 0.3f;

        public void SetWorld(EcsWorld world, int entity, SceneData sd)
        {
            _world = world;
            _ownerEntity = entity;
            _hasItems = _world.GetPool<HasItems>();
            _itemsPool = _world.GetPool<ItemComp>();
            _inventoryPool = _world.GetPool<InventoryComp>();
            _chestPool = _world.GetPool<ChestComp>();
            _playerPool = _world.GetPool<PlayerComp>();
            _deletePool = _world.GetPool<DeleteEvent>();
            _sd = sd;
        }

        private void Start()
        {
            _parentBeforeDrag = transform.parent;
        }

        private void Update()
        {
            _deleteFilter = _world.Filter<DeleteEvent>().End();
            foreach (var entity in _deleteFilter)
            {
                ref var deleteEvent = ref _deletePool.Get(entity);

                if (deleteEvent.Result)
                {
                    if (_isDeleteEvent)
                    {
                        DestroyItem();
                        _isDeleteEvent = false;
                    }

                    _deleteFormView.gameObject.SetActive(false);
                    _crosshairView.gameObject.SetActive(true);
                }
                else
                {
                    if (_ownerEntity == _playerInventoryViewContent.currentEntity)
                        MoveItemTo(_playerInventoryViewContent.currentEntity, _playerInventoryViewContent.transform);
                    else if (_ownerEntity == _chestInventoryViewContent.currentEntity)
                        MoveItemTo(_chestInventoryViewContent.currentEntity, _chestInventoryViewContent.transform);

                    _deleteFormView.gameObject.SetActive(false);
                    _crosshairView.gameObject.SetActive(true);
                }
            }
        }

        public void SetViews(RectTransform playerInventoryView, RectTransform chestInventoryView,
            RectTransform fastItemsView, RectTransform deleteFormView, RectTransform crosshairView)
        {
            _playerInventoryView = playerInventoryView;
            _chestInventoryView = chestInventoryView;
            _fastItemsView = fastItemsView;
            _crosshairView = crosshairView;

            _playerInventoryViewContent = playerInventoryView.GetComponentInChildren<ContentView>();
            _chestInventoryViewContent = chestInventoryView.GetComponentInChildren<ContentView>();
            _deleteFormView = deleteFormView.GetComponent<DeleteFormView>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Time.time - _lastClickTime <= _doubleClickThreshold)
                    MoveItemTo(_playerInventoryViewContent.currentEntity, _playerInventoryViewContent.transform);

                _lastClickTime = Time.time;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!itemObject)
                    return;

                ref var hasItems = ref _hasItems.Get(_ownerEntity);

                ItemIdx.Unpack(_world, out var currentEntity);

                foreach (var itemPacked in hasItems.Entities)
                    if (itemPacked.Unpack(_world, out var unpackedEntity))
                    {
                        if (currentEntity == unpackedEntity)
                            itemObject.gameObject.SetActive(!itemObject.gameObject.activeSelf);
                        else
                            itemObject.gameObject.SetActive(false);
                    }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _parentBeforeDrag = transform.parent;
                transform.SetParent(transform.root);
                transform.SetAsLastSibling();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                transform.position = Mouse.current.position.value;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (IsItemOutInventory(transform.position, _playerInventoryView) ||
                IsItemOutInventory(transform.position, _chestInventoryView))
            {
                if (IsItemInsideInventory(transform.position, _fastItemsView))
                {
                    SetFastItemView();
                    if (_ownerEntity == _playerInventoryViewContent.currentEntity)
                    {
                        transform.SetParent(_parentBeforeDrag);
                    }
                    else
                    {
                        MoveItemTo(_playerInventoryViewContent.currentEntity, _playerInventoryViewContent.transform);
                        var playerComp = _playerPool.Get(_playerInventoryViewContent.currentEntity);
                        var rot = itemObject.transform.localRotation;
                        itemObject.transform.SetParent(playerComp.Transform);
                        itemObject.transform.localRotation = rot;
                        itemObject.transform.position =
                            playerComp.Transform.localPosition + playerComp.Transform.forward;
                    }
                }
                else if (IsItemInsideInventory(transform.position, _chestInventoryView))
                {
                    MoveItemTo(_chestInventoryViewContent.currentEntity, _chestInventoryViewContent.transform);
                }
                else if (IsItemInsideInventory(transform.position, _playerInventoryView))
                {
                    MoveItemTo(_playerInventoryViewContent.currentEntity, _playerInventoryViewContent.transform);
                }
                else
                {
                    _deleteFormView.gameObject.SetActive(true);
                    _crosshairView.gameObject.SetActive(false);
                    _isDeleteEvent = true;
                }
            }
            else
            {
                transform.SetParent(_parentBeforeDrag);
            }
        }

        private void SetFastItemView()
        {
            foreach (var ft in _sd.fastItemViews)
                if (IsCursorOver(ft))
                {
                    if (ft.itemObject != null)
                    {
                        ft.itemObject = itemObject;
                        ft.itemObject.ItemIdx = ItemIdx;
                    }
                    ft.itemImage.sprite = itemImage.sprite;
                    ft.itemName.text = ItemName;
                    ft.itemCount.text = ItemCount;
                    return;
                }
        }

        private bool IsCursorOver(FastItemView ft)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(), ft);
        }

        private FastItemView IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, FastItemView ft)
        {
            return eventSystemRaysastResults
                .Select(curRaysastResult => curRaysastResult.gameObject.GetComponentInParent<FastItemView>())
                .FirstOrDefault(targetComp => targetComp && targetComp == ft);
        }

        private List<RaycastResult> GetEventSystemRaycastResults()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }


        private void MoveItemTo(int otherEntity, Transform newParent)
        {
            if (otherEntity == _ownerEntity)
            {
                transform.SetParent(_parentBeforeDrag);
                return;
            }

            ref var hasItemsOwner = ref _hasItems.Get(_ownerEntity);
            ref var hasItemsOther = ref _hasItems.Get(otherEntity);

            ref var ownerInventory = ref _inventoryPool.Get(_ownerEntity);
            ref var otherInventory = ref _inventoryPool.Get(otherEntity);

            ItemIdx.Unpack(_world, out var unpackedEntity);
            ref var item = ref _itemsPool.Get(unpackedEntity);

            if (otherEntity == _chestInventoryViewContent.currentEntity)
            {
                ref var chestComp = ref _chestPool.Get(otherEntity);

                chestComp.ChestObject.itemViews.Add(this);
            }

            ownerInventory.CurrentWeight -= item.Weight;
            otherInventory.CurrentWeight += item.Weight;

            ownerInventory.InventoryWeightView.inventoryWeightText.text =
                $"Вес: {ownerInventory.CurrentWeight}/{ownerInventory.MaxWeight}";
            otherInventory.InventoryWeightView.inventoryWeightText.text =
                $"Вес: {otherInventory.CurrentWeight}/{otherInventory.MaxWeight}";

            hasItemsOther.Entities.Add(ItemIdx);
            hasItemsOwner.Entities.Remove(ItemIdx);

            transform.SetParent(newParent);
            _ownerEntity = otherEntity;
        }

        private void DestroyItem()
        {
            if (ItemIdx.Unpack(_world, out var unpackedEntity))
            {
                foreach (var ft in _sd.fastItemViews)
                    if (ft.itemObject && ft.itemObject.ItemIdx.Unpack(_world, out var ftUnpackedEntity))
                        if (ftUnpackedEntity == unpackedEntity)
                        {
                            ft.itemObject = null;
                            ft.itemImage.sprite = null;
                            ft.itemName.text = "";
                            ft.itemCount.text = "";
                            break;
                        }

                ref var item = ref _itemsPool.Get(unpackedEntity);

                ref var inventory = ref _inventoryPool.Get(_ownerEntity);
                inventory.CurrentWeight -= item.Weight;
                inventory.InventoryWeightView.inventoryWeightText.text =
                    $"Вес: {inventory.CurrentWeight}/{inventory.MaxWeight}";

                Destroy(itemObject != null ? itemObject.gameObject : null);

                _itemsPool.Del(unpackedEntity);
            }

            Destroy(transform.gameObject);
        }

        private bool IsItemOutInventory(Vector3 position, RectTransform view)
        {
            var minPosition = view.TransformPoint(view.rect.min);
            var maxPosition = view.TransformPoint(view.rect.max);

            return position.x < minPosition.x || position.x > maxPosition.x || position.y < minPosition.y ||
                   position.y > maxPosition.y;
        }

        private bool IsItemInsideInventory(Vector3 position, RectTransform view)
        {
            if (!view.gameObject.activeInHierarchy)
                return false;

            var minPosition = view.TransformPoint(view.rect.min);
            var maxPosition = view.TransformPoint(view.rect.max);

            return position.x >= minPosition.x && position.x <= maxPosition.x &&
                   position.y >= minPosition.y && position.y <= maxPosition.y;
        }
    }
}