using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Inventory;
using World.Ability;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<PlayerComp> _playerPool = default;
        private readonly EcsPoolInject<PlayerInputComp> _playerInputPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;
        private readonly EcsPoolInject<LevelComp> _levelPool = default;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Init(IEcsSystems systems)
        {
            var playerEntity = _world.Value.NewEntity();
            var playerPacked = _world.Value.PackEntity(playerEntity);

            ref var player = ref _playerPool.Value.Add(playerEntity);
            ref var rpg = ref _rpgPool.Value.Add(playerEntity);
            ref var level = ref _levelPool.Value.Add(playerEntity);
            _inventoryPool.Value.Add(playerEntity);
            _playerInputPool.Value.Add(playerEntity);

            var playerPrefab = _cf.Value.playerConfiguration.playerPrefab;
            var playerFollowCameraPrefab = _cf.Value.playerConfiguration.playerFollowCameraPrefab;
            var playerStartPosition = _sd.Value.playerSpawnPosition.position;
            var playerObject = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            var playerFollowCameraView =
                Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

            player.Transform = playerObject.transform;
            player.Position = playerStartPosition;
            player.Rotation = Quaternion.identity;
            player.CharacterController = playerObject.GetComponent<CharacterController>();
            player.PlayerCameraRoot = playerObject.GetComponentInChildren<PlayerCameraRootView>().transform;
            player.Grounded = true;
            player.PlayerCamera = playerFollowCameraView;

            var playerView = player.Transform.GetComponentInChildren<PlayerView>();
            playerView.PlayerPacked = playerPacked;

            playerFollowCameraView.Follow = player.PlayerCameraRoot;

            rpg.Health = _cf.Value.playerConfiguration.health;
            rpg.Stamina = _cf.Value.playerConfiguration.stamina;
            rpg.Mana = _cf.Value.playerConfiguration.mana;
            rpg.CanRun = true;
            rpg.CanDash = true;
            rpg.CanJump = true;

            level.Level = _cf.Value.playerConfiguration.startLevel;
            level.Experience = _cf.Value.playerConfiguration.startExperience;
            level.ExperienceToNextLevel = _cf.Value.playerConfiguration.experienceToNextLevel[0];
        }
    }
}