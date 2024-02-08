using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Configurations;
using World.Inventory;
using World.LoadGame;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<LoadDataEventComp>> _loadDataFilter = Idents.Worlds.Events;
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
            foreach (var loadDataEventEntity in _loadDataFilter.Value)
            {
                ref var loadDataEventComp = ref _loadDataFilter.Pools.Inc1.Get(loadDataEventEntity);
                
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
                var playerStartPosition = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.PlayerData.Position : _sd.Value.playerSpawnPosition.position;
                var playerObject = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
                var playerFollowCameraView =
                    Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

                playerComp.Transform = playerObject.transform;
                playerComp.Position = playerStartPosition;
                playerComp.Rotation = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.PlayerData.Rotation : Quaternion.identity;
                playerComp.CharacterController = playerObject.GetComponent<CharacterController>();
                playerComp.PlayerCameraRootTransform =
                    playerObject.GetComponentInChildren<PlayerCameraRootView>().transform;
                playerComp.PlayerCameraStatsTransform =
                    playerObject.GetComponentInChildren<PlayerCameraStatsView>().transform;
                playerComp.Grounded = !loadDataEventComp.IsLoadData || loadDataEventComp.PlayerSaveData.PlayerData.Grounded;
                playerComp.PlayerCameraRoot = playerFollowCameraView;
                playerComp.PlayerCameraStats =
                    playerComp.PlayerCameraStatsTransform.GetComponent<CinemachineVirtualCamera>();
                playerComp.CanMove = !loadDataEventComp.IsLoadData || loadDataEventComp.PlayerSaveData.PlayerData.CanMove;
                playerComp.CameraSense = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.PlayerData.CameraSense : _cf.Value.playerConfiguration.deltaTimeMultiplier;
                playerComp.GoldAmount = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.PlayerData.GoldAmount : _cf.Value.playerConfiguration.startPlayerGold;
                playerComp.VerticalVelocity = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.PlayerData.VerticalVelocity : 0f;
                playerComp.IsWalking = loadDataEventComp.IsLoadData && loadDataEventComp.PlayerSaveData.PlayerData.IsWalking;

                var coinsAmount = playerComp.GoldAmount.ToString();
                _sd.Value.uiSceneData.playerInventoryGoldAmount.goldAmount.text = coinsAmount;
                _sd.Value.uiSceneData.traderShopView.playerCoins.text = "Мои монеты: " + coinsAmount;

                animationComp.Animator = playerObject.GetComponentInChildren<Animator>();

                var playerView = playerComp.Transform.GetComponentInChildren<PlayerView>();
                playerView.PlayerPacked = playerPacked;

                playerFollowCameraView.Follow = playerComp.PlayerCameraRootTransform;
                playerFollowCameraView.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance =
                    _cf.Value.playerConfiguration.minZoomDistance;

                rpgComp.Health = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.RpgData.Health : _cf.Value.playerConfiguration.health;
                rpgComp.Stamina = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.RpgData.Stamina : _cf.Value.playerConfiguration.stamina;
                rpgComp.Mana = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.RpgData.Mana : _cf.Value.playerConfiguration.mana;
                rpgComp.CanRun = !loadDataEventComp.IsLoadData || loadDataEventComp.PlayerSaveData.RpgData.CanRun;
                rpgComp.CanDash = !loadDataEventComp.IsLoadData || loadDataEventComp.PlayerSaveData.RpgData.CanDash;
                rpgComp.CanJump = !loadDataEventComp.IsLoadData || loadDataEventComp.PlayerSaveData.RpgData.CanJump;

                levelComp.Level = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.LevelData.Level : _cf.Value.playerConfiguration.startLevel;
                levelComp.Experience = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.LevelData.Experience : _cf.Value.playerConfiguration.startExperience;
                levelComp.ExperienceToNextLevel = loadDataEventComp.IsLoadData ? loadDataEventComp.PlayerSaveData.LevelData.ExperienceToNextLevel : _cf.Value.playerConfiguration.experienceToNextLevel[0];
            }
        }
    }
}