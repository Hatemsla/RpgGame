using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Inventory;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<PlayerComp> _playerPool = default;
        private readonly EcsPoolInject<PlayerInputComp> _playerInputPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<HasItems> _hasItemsPool = default;
        private readonly EcsPoolInject<Item> _itemsPool = default;

        private readonly EcsCustomInject<SceneData> _sc = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var playerEntity = world.NewEntity();

            ref var player = ref _playerPool.Value.Add(playerEntity);
            ref var rpg = ref _rpgPool.Value.Add(playerEntity);
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
            player.PlayerWeaponRoot = playerView.GetComponentInChildren<ItemView>().transform;
            player.Grounded = true;
            player.PlayerCamera = playerFollowCameraView;

            playerFollowCameraView.Follow = player.PlayerCameraRoot;

            rpg.Health = _cf.Value.playerConfiguration.health;
            rpg.Stamina = _cf.Value.playerConfiguration.stamina;
            rpg.Mana = _cf.Value.playerConfiguration.mana;
            rpg.CanRun = true;
            rpg.CanDash = true;
            rpg.CanJump = true;

            CreateItems(playerEntity, world);
        }

        private void CreateItems(int entity, EcsWorld world)
        {
            ref var hasItems = ref _hasItemsPool.Value.Add(entity);
            var items = _cf.Value.inventoryConfiguration.items;

            var i = 0;
            foreach (var item in items)
            {
                var itemEntity = world.NewEntity();
                ref var it = ref _itemsPool.Value.Add(itemEntity);
                it.ItemName = item.itemName;
                it.ItemDescription = item.itemDescription;
                it.Cost = item.cost;
                
                var itemView = Object.Instantiate(item.itemViewPrefab,
                    _playerPool.Value.Get(entity).Transform.position + _playerPool.Value.Get(entity).Transform.forward,
                    item.itemViewPrefab.transform.rotation);
                itemView.transform.SetParent(_sc.Value.playerTransform);
                itemView.gameObject.SetActive(false);

                it.ItemView = itemView;
                it.ItemView.itemIdx = i++;
                
                hasItems.Entities.Add(itemEntity);
            }
        }
    }
}