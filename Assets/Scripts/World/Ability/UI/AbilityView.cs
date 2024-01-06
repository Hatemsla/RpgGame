using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using World.Ability.AbilitiesObjects;
using World.Inventory;
using World.Player;

namespace World.Ability.UI
{
    public class AbilityView : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public EcsPackedEntity AbilityIdx;
        public Image abilityImage;
        public AbilityObject abilityObject;
        
        [SerializeField] private TMP_Text abilityName;
        [SerializeField] private TMP_Text abilityDescription;
        [SerializeField] private TMP_Text abilityParams;

        //private ContentView _playerSkillViewContent;
        
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
        
        public string AbilityParams
        {
            get => abilityParams.text;
            set => abilityParams.text = value;
        }
        
        private EcsWorld _world;
        private int _ownerEntity;
        private EcsPool<HasAbilities> _hasAbilities;
        private EcsPool<AbilityComp> _abilityPool;
        private EcsPool<PlayerComp> _playerPool;
        
        private ContentView _playerAbilityViewContent;
        
        private RectTransform _crosshairView;
        private RectTransform _playerAbilityView;
        private RectTransform _fastSkillsView;

        private float _lastClickTime;
        private readonly float _doubleClickThreshold = 0.3f;
        
        public void SetWorld(EcsWorld world, int entity, SceneData sd)
        {
            _world = world;
            _ownerEntity = entity;
            _hasAbilities = _world.GetPool<HasAbilities>();
            _abilityPool = _world.GetPool<AbilityComp>();
            _playerPool = _world.GetPool<PlayerComp>();
            _sd = sd;
        }
        
        private void Start()
        {
            _parentBeforeDrag = transform.parent;
        }

        public void SetViews(RectTransform playerAbilityView, RectTransform fastSkillsView, RectTransform crosshairView)
        {
            _playerAbilityView = playerAbilityView;
            _fastSkillsView = fastSkillsView;
            _crosshairView = crosshairView;

            _playerAbilityViewContent = playerAbilityView.GetComponentInChildren<ContentView>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (Time.time - _lastClickTime <= _doubleClickThreshold)
                    MoveSkillTo(_playerAbilityViewContent.currentEntity, _playerAbilityViewContent.transform);

                _lastClickTime = Time.time;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!abilityObject)
                    return;

                ref var hasAbilities = ref _hasAbilities.Get(_ownerEntity);

                AbilityIdx.Unpack(_world, out var currentEntity);

                foreach (var abilityPacked in hasAbilities.Entities)
                    if (abilityPacked.Unpack(_world, out var unpackedEntity))
                    {
                        if (currentEntity == unpackedEntity)
                            abilityObject.gameObject.SetActive(!abilityObject.gameObject.activeSelf);
                        else
                            abilityObject.gameObject.SetActive(false);
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
            if (IsSkillOutList(transform.position, _playerAbilityView))
            {
                if (IsSkillInsideList(transform.position, _fastSkillsView))
                {
                    SetFastSkillView();
                    if (_ownerEntity == _playerAbilityViewContent.currentEntity)
                    {
                        transform.SetParent(_parentBeforeDrag);
                    }
                    else
                    {
                        MoveSkillTo(_playerAbilityViewContent.currentEntity, _playerAbilityViewContent.transform);
                        var playerComp = _playerPool.Get(_playerAbilityViewContent.currentEntity);
                        var rot = abilityObject.transform.localRotation;
                        abilityObject.transform.SetParent(playerComp.Transform);
                        abilityObject.transform.localRotation = rot;
                        abilityObject.transform.position =
                            playerComp.Transform.localPosition + playerComp.Transform.forward;
                    }
                }
                else if (IsSkillInsideList(transform.position, _playerAbilityView))
                {
                    MoveSkillTo(_playerAbilityViewContent.currentEntity, _playerAbilityViewContent.transform);
                }
            }
            else
            {
                transform.SetParent(_parentBeforeDrag);
            }
        }
        
        private void SetFastSkillView()
        {
            foreach (var fs in _sd.fastSkillViews)
                if (IsCursorOver(fs))
                {
                    fs.abilityObject = abilityObject;
                    fs.abilityObject.AbilityIdx = AbilityIdx;
                    fs.abilityImage.sprite = abilityImage.sprite;
                    fs.abilityName.text = AbilityName;
                    return;
                }
        }
        
        private bool IsCursorOver(FastSkillView fs)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(), fs);
        }
        
        private FastSkillView IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, FastSkillView fs)
        {
            return eventSystemRaysastResults
                .Select(curRaysastResult => curRaysastResult.gameObject.GetComponentInParent<FastSkillView>())
                .FirstOrDefault(targetComp => targetComp && targetComp == fs);
        }
        
        private List<RaycastResult> GetEventSystemRaycastResults()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
        
        private void MoveSkillTo(int otherEntity, Transform newParent)
        {
            if (otherEntity == _ownerEntity)
            {
                transform.SetParent(_parentBeforeDrag);
                return;
            }

            ref var hasSkillsOwner = ref _hasAbilities.Get(_ownerEntity);
            ref var hasSkillsOther = ref _hasAbilities.Get(otherEntity);
            
            AbilityIdx.Unpack(_world, out var unpackedEntity);
            ref var ability = ref _abilityPool.Get(unpackedEntity);
            
            hasSkillsOther.Entities.Add(AbilityIdx);
            hasSkillsOwner.Entities.Remove(AbilityIdx);
            
            transform.SetParent(newParent);
            _ownerEntity = otherEntity;
        }
        
        private bool IsSkillOutList(Vector3 position, RectTransform view)
        {
            var minPosition = view.TransformPoint(view.rect.min);
            var maxPosition = view.TransformPoint(view.rect.max);

            return position.x < minPosition.x || position.x > maxPosition.x || position.y < minPosition.y ||
                   position.y > maxPosition.y;
        }

        private bool IsSkillInsideList(Vector3 position, RectTransform view)
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