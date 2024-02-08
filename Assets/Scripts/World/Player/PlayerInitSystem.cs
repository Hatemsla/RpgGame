using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.Inventory;
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
        private readonly EcsPoolInject<AnimationComp> _animationPool = default;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Init(IEcsSystems systems)
        {
            var playerEntity = _world.Value.NewEntity();
            var playerPacked = _world.Value.PackEntity(playerEntity);

            ref var playerComp = ref _playerPool.Value.Add(playerEntity);
            ref var rpgComp = ref _rpgPool.Value.Add(playerEntity);
            ref var levelComp = ref _levelPool.Value.Add(playerEntity);
            ref var animationComp = ref _animationPool.Value.Add(playerEntity);
            _inventoryPool.Value.Add(playerEntity);
            _playerInputPool.Value.Add(playerEntity);

            var playerPrefab = _cf.Value.playerConfiguration.playerPrefab;
            var playerFollowCameraPrefab = _cf.Value.playerConfiguration.playerFollowCameraPrefab;
            var playerStartPosition = _sd.Value.playerSpawnPosition.position;
            var playerObject = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            var playerFollowCameraView =
                Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

            playerComp.Transform = playerObject.transform;
            playerComp.Position = playerStartPosition;
            playerComp.Rotation = Quaternion.identity;
            playerComp.CharacterController = playerObject.GetComponent<CharacterController>();
            playerComp.PlayerCameraRootTransform = playerObject.GetComponentInChildren<PlayerCameraRootView>().transform;
            playerComp.PlayerCameraStatsTransform = playerObject.GetComponentInChildren<PlayerCameraStatsView>().transform;
            playerComp.Grounded = true;
            playerComp.PlayerCameraRoot = playerFollowCameraView;
            playerComp.PlayerCameraStats = playerComp.PlayerCameraStatsTransform.GetComponent<CinemachineVirtualCamera>();
            playerComp.CanMove = true;
            playerComp.CameraSense = _cf.Value.playerConfiguration.deltaTimeMultiplier;
            playerComp.GoldAmount = _cf.Value.playerConfiguration.startPlayerGold;
            
            var coinsAmount = playerComp.GoldAmount.ToString();
            _sd.Value.uiSceneData.playerInventoryGoldAmount.goldAmount.text = coinsAmount;
            _sd.Value.uiSceneData.traderShopView.playerCoins.text = "Мои монеты: " + coinsAmount;

            animationComp.Animator = playerObject.GetComponentInChildren<Animator>();

            var playerView = playerComp.Transform.GetComponentInChildren<PlayerView>();
            playerView.PlayerPacked = playerPacked;

            playerFollowCameraView.Follow = playerComp.PlayerCameraRootTransform;
            playerFollowCameraView.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance =
                _cf.Value.playerConfiguration.minZoomDistance;

            rpgComp.Health = _cf.Value.playerConfiguration.health;
            rpgComp.Stamina = _cf.Value.playerConfiguration.stamina;
            rpgComp.Mana = _cf.Value.playerConfiguration.mana;
            rpgComp.CanRun = true;
            rpgComp.CanDash = true;
            rpgComp.CanJump = true;

            levelComp.Level = _cf.Value.playerConfiguration.startLevel;
            levelComp.Experience = _cf.Value.playerConfiguration.startExperience;
            levelComp.ExperienceToNextLevel = _cf.Value.playerConfiguration.experienceToNextLevel[0];
        }
    }
}