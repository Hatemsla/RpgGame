using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using TMPro;
using UnityEngine;
using Utils;
using World.Inventory;
using World.Ability;
using World.Configurations;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<PlayerComp> _playerPool = default;
        private readonly EcsPoolInject<PlayerInputComp> _playerInputPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<ItemComp> _itemsPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;
        private readonly EcsPoolInject<AbilityComp> _ability = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<SceneData> _sc = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        
        [EcsUguiNamed(Idents.UI.PlayerInventoryView)]
        private readonly RectTransform _playerInventoryView = default;
        
        [EcsUguiNamed(Idents.UI.ChestInventoryView)]
        private readonly RectTransform _chestInventoryView = default;

        [EcsUguiNamed(Idents.UI.PlayerInventoryWeight)]
        private readonly Transform _inventoryWeightText = default;

        public void Init(IEcsSystems systems)
        {
            var playerEntity = _world.Value.NewEntity();

            ref var player = ref _playerPool.Value.Add(playerEntity);
            ref var rpg = ref _rpgPool.Value.Add(playerEntity);
            ref var inventory = ref _inventoryPool.Value.Add(playerEntity);
            _playerInputPool.Value.Add(playerEntity);

            var playerPrefab = _cf.Value.playerConfiguration.playerPrefab;
            var playerFollowCameraPrefab = _cf.Value.playerConfiguration.playerFollowCameraPrefab;
            var playerStartPosition = _sc.Value.playerSpawnPosition.position;
            var playerView = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            var playerFollowCameraView =
                Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

            _sc.Value.playerTransform = playerView.transform;
            
            player.Transform = playerView.transform;
            player.Position = playerStartPosition;
            player.Rotation = Quaternion.identity;
            player.CharacterController = playerView.GetComponent<CharacterController>();
            player.PlayerCameraRoot = playerView.GetComponentInChildren<PlayerCameraRootView>().transform;
            player.PlayerWeaponRoot = playerView.GetComponentInChildren<PlayerView>().transform;
            player.Grounded = true;
            player.PlayerCamera = playerFollowCameraView;

            playerFollowCameraView.Follow = player.PlayerCameraRoot;

            rpg.Health = _cf.Value.playerConfiguration.health;
            rpg.Stamina = _cf.Value.playerConfiguration.stamina;
            rpg.Mana = _cf.Value.playerConfiguration.mana;
            rpg.CanRun = true;
            rpg.CanDash = true;
            rpg.CanJump = true;

            inventory.MaxWeight = _cf.Value.inventoryConfiguration.inventoryWeight;
            inventory.CurrentWeight = 0f;
            
            _playerInventoryView.gameObject.SetActive(false);

			CreateAbilities(playerEntity, _world.Value);
            inventory.CurrentWeight = CreateItems(playerEntity, _world.Value);

            inventory.InventoryWeightView = _inventoryWeightText.GetComponent<InventoryWeightView>();
            inventory.InventoryWeightView.inventoryWeightText.text = $"Вес: {inventory.CurrentWeight}/{inventory.MaxWeight}";
        }

        private float CreateItems(int entity, EcsWorld world)
        {
            ref var hasItems = ref _hasItemsPool.Value.Add(entity);
            var items = _cf.Value.inventoryConfiguration.items;
            var playerInventoryViewContent = _playerInventoryView.GetComponentInChildren<ContentView>();
            playerInventoryViewContent.currentEntity = entity;

            var weight = 0f;
            foreach (var itemData in items)
            {
                var itemEntity = world.NewEntity();
                var itemPackedEntity = world.PackEntity(itemEntity);
                ref var it = ref _itemsPool.Value.Add(itemEntity);
                
                it.ItemName = itemData.itemName;
                it.ItemDescription = itemData.itemDescription;
                it.Cost = itemData.cost;
                it.Weight = itemData.itemWeight;
                
                weight += itemData.itemWeight;
                
                var itemObject = Object.Instantiate(itemData.itemObjectPrefab,
                    _playerPool.Value.Get(entity).Transform.position + _playerPool.Value.Get(entity).Transform.forward,
                    itemData.itemObjectPrefab.transform.rotation);
                itemObject.transform.SetParent(_sc.Value.playerTransform);
                itemObject.gameObject.SetActive(false);

                var itemView = Object.Instantiate(itemData.itemViewPrefab, Vector3.zero, Quaternion.identity);
                itemView.transform.SetParent(playerInventoryViewContent.transform);
                it.ItemView = itemView;

                it.ItemView.itemImage.sprite = itemData.itemSprite;
                
                it.ItemView.itemObject = itemObject;
                it.ItemView.itemObject.itemIdx = itemPackedEntity;
                it.ItemView.ItemIdx = itemPackedEntity;
                it.ItemView.ItemName = itemData.itemName;
                it.ItemView.ItemDescription = itemData.itemDescription;
                it.ItemView.ItemCount = itemData.itemCount.ToString();
                it.ItemView.SetWorld(world, entity);
                it.ItemView.SetViews(_playerInventoryView, _chestInventoryView);

                hasItems.Entities.Add(itemPackedEntity);
            }

            return weight;
        }

        private void CreateAbilities(int playerEntity, EcsWorld world)
        {
            ref var hasAbilities = ref _hasAbilitiesPool.Value.Add(playerEntity);
            foreach (var ability in _cf.Value.abilityConfiguration.abilityDatas)
            {
                if (ability.name == Idents.Abilities.FireBall)
                {
                    var abilityEntity = world.NewEntity();
                    var abilityPackedEntity = world.PackEntity(abilityEntity);
                    ref var abil = ref _ability.Value.Add(abilityEntity);
                    
                    abil.Name = ability.name;
                    abil.Damage = ability.damage;
                    abil.LifeTime = ability.lifeTime;
                    abil.Radius = ability.radius;
                    abil.Speed = ability.speed;
                    abil.CostPoint = ability.costPoint;
                    
                    hasAbilities.Entities.Add(abilityPackedEntity);
                }
            }
        }
    }
}