using System.Collections.Generic;
using System.Linq;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using World.Ability.AbilitiesObjects;
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
        
        private RectTransform _crosshairView;
        private RectTransform _fastSkillsView;
        
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
        
        public void SetViews(RectTransform fastSkillsView, RectTransform crosshairView)
        {
            _fastSkillsView = fastSkillsView;
            _crosshairView = crosshairView;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                /*if (Time.time - _lastClickTime <= _doubleClickThreshold)
                    MoveSkillTo(_playerSkillViewContent.currentEntity, _playerSkillViewContent.transform);*/

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
            if (IsSkillInsideInventory(transform.position, _fastSkillsView))
            {
                SetFastSkillView();
                /*if (_ownerEntity == _playerInventoryViewContent.currentEntity)
                {
                    transform.SetParent(_parentBeforeDrag);
                }
                else
                {
                    MoveSkillTo(_playerInventoryViewContent.currentEntity, _playerInventoryViewContent.transform);
                    var playerComp = _playerPool.Get(_playerInventoryViewContent.currentEntity);
                    var rot = itemObject.transform.localRotation;
                    itemObject.transform.SetParent(playerComp.Transform);
                    itemObject.transform.localRotation = rot;
                    itemObject.transform.position =
                        playerComp.Transform.localPosition + playerComp.Transform.forward;
                }*/
            }
        }
        
        private void SetFastSkillView()
        {
            foreach (var fs in _sd.fastSkillViews)
                if (IsCursorOver(fs))
                {
                    fs.abilityImage.sprite = abilityImage.sprite;
                    fs.abilityObject = abilityObject;
                    fs.abilityName.text = AbilityName;
                    return;
                }
        }
        
        private bool IsCursorOver(FastSkillView fs)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(), fs);
        }
        
        private FastSkillView IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, FastSkillView ft)
        {
            return eventSystemRaysastResults
                .Select(curRaysastResult => curRaysastResult.gameObject.GetComponentInParent<FastSkillView>())
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
        
        private void MoveSkillTo(int otherEntity, Transform newParent)
        {
            if (otherEntity == _ownerEntity)
            {
                transform.SetParent(_parentBeforeDrag);
                return;
            }

            ref var hasSkillsOwner = ref _hasAbilities.Get(_ownerEntity);
            ref var hasSkillsOther = ref _hasAbilities.Get(otherEntity);
            
            transform.SetParent(newParent);
            _ownerEntity = otherEntity;
        }
        
        private bool IsSkillOutInventory(Vector3 position, RectTransform view)
        {
            var minPosition = view.TransformPoint(view.rect.min);
            var maxPosition = view.TransformPoint(view.rect.max);

            return position.x < minPosition.x || position.x > maxPosition.x || position.y < minPosition.y ||
                   position.y > maxPosition.y;
        }

        private bool IsSkillInsideInventory(Vector3 position, RectTransform view)
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