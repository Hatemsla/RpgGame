using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Utils;
using World.Inventory;
using World.Ability;
using World.Configurations;
using World.Network;
using Object = UnityEngine.Object;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<PlayerComp> _playerPool = default;
        private readonly EcsPoolInject<NetworkComp> _networkPool = default;
        private readonly EcsPoolInject<PlayerInputComp> _playerInputPool = default;
        private readonly EcsPoolInject<RpgComp> _rpgPool = default;
        private readonly EcsPoolInject<InventoryComp> _inventoryPool = default;
        private readonly EcsPoolInject<AbilityComp> _ability = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;

        private readonly EcsWorldInject _world = default;

        private readonly EcsCustomInject<SceneData> _sd = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<NetworkRunnerService> _nrs = default;

        public void Init(IEcsSystems systems)
        {
            Utils.Utils.DebugLog("PlayerInitSystem");

            var playerEntity = _world.Value.NewEntity();
            var playerPacked = _world.Value.PackEntity(playerEntity);

            ref var playerComp = ref _playerPool.Value.Add(playerEntity);
            ref var rpgComp = ref _rpgPool.Value.Add(playerEntity);
            ref var networkComp = ref _networkPool.Value.Add(playerEntity);
            _inventoryPool.Value.Add(playerEntity);
            _playerInputPool.Value.Add(playerEntity);

            var playerPrefab = _cf.Value.playerConfiguration.playerPrefab;
            var playerFollowCameraPrefab = _cf.Value.playerConfiguration.playerFollowCameraPrefab;
            var playerStartPosition = _sd.Value.playerSpawnPosition.position;
            // var playerObject = runner.Spawn(playerPrefab, playerStartPosition, Quaternion.identity, player);
            var playerObject = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            var playerFollowCameraView =
                Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

            playerComp.Transform = playerObject.transform;
            playerComp.Position = playerStartPosition;
            playerComp.Rotation = Quaternion.identity;
            playerComp.CharacterController = playerObject.GetComponent<CharacterController>();
            playerComp.PlayerCameraRoot = playerObject.GetComponentInChildren<PlayerCameraRootView>().transform;
            playerComp.Grounded = true;
            playerComp.PlayerCamera = playerFollowCameraView;

            var playerView = playerComp.Transform.GetComponentInChildren<PlayerView>();
            playerView.PlayerPacked = playerPacked;

            playerFollowCameraView.Follow = playerComp.PlayerCameraRoot;

            rpgComp.Health = _cf.Value.playerConfiguration.health;
            rpgComp.Stamina = _cf.Value.playerConfiguration.stamina;
            rpgComp.Mana = _cf.Value.playerConfiguration.mana;
            rpgComp.CanRun = true;
            rpgComp.CanDash = true;
            rpgComp.CanJump = true;

            CreateAbilities(playerEntity, _world.Value);
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
                    abil.Distance = ability.distance;
                    abil.Speed = ability.speed;
                    abil.CostPoint = ability.costPoint;
                    
                    hasAbilities.Entities.Add(abilityPackedEntity);
                }
            }
        }
    }
}