using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using World.Inventory;
using World.Inventory.Chest;
using World.Player;

namespace World.Ability
{
    public class AbilityView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public EcsPackedEntity AbilityIdx;
        public Image abilityImage;
        
        [SerializeField] private TMP_Text abilityName;
        [SerializeField] private TMP_Text abilityDescription;
        [SerializeField] private TMP_Text abilityParametrs;
        
        private Transform _parentBeforeDrag;
        private SceneData _sd;
        
        
        public string AbilityName
        {
            get => abilityName.text;
            set => abilityName.text = value;
        }
        
        public string AbilityDescription
        {
            get => abilityDescription.text;
            set => abilityDescription.text = value;
        }
        
        public string AabilityParametrs
        {
            get => abilityParametrs.text;
            set => abilityParametrs.text = value;
        }
        
        private EcsWorld _world;
        private int _ownerEntity;
        private EcsPool<HasAbilities> _hasAbilities;
        private EcsPool<AbilityComp> _abilityComp;
        private EcsPool<PlayerComp> _playerPool;
        private EcsFilter _deleteFilter;
        
        private RectTransform _crosshairView;
        private RectTransform _fastItemsView;
        
        private float _lastClickTime;
        private readonly float _doubleClickThreshold = 0.3f;
        
        public void SetWorld(EcsWorld world, int entity, SceneData sd)
        {
            _world = world;
            _ownerEntity = entity;
            _hasAbilities = _world.GetPool<HasAbilities>();
            _playerPool = _world.GetPool<PlayerComp>();
            _sd = sd;
        }
        
        private void Start()
        {
            _parentBeforeDrag = transform.parent;
        }
        
        public void SetViews(RectTransform playerInventoryView, RectTransform chestInventoryView,
            RectTransform fastItemsView, RectTransform deleteFormView, RectTransform crosshairView)
        {
            _fastItemsView = fastItemsView;
            _crosshairView = crosshairView;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            /*if (eventData.button == PointerEventData.InputButton.Left)
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
            }*/
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
        
        private void SetFastSkillView()
        {
            foreach (var ft in _sd.fastSkillViews)
                if (IsCursorOver(ft))
                {
                    ft.abilityImage.sprite = abilityImage.sprite;
                    ft.abilityName.text = AbilityName;
                    return;
                }
        }
        
        private bool IsCursorOver(FastSkillView ft)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(), ft);
        }
        
        private FastItemView IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, FastSkillView ft)
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
    }
}