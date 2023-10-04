using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using Utils;
using World.Ability;

namespace World.Player
{
    public sealed class PlayerInitSystem : IEcsInitSystem
    {
        private readonly EcsPoolInject<PlayerComp> _playerPool = default;
        private readonly EcsPoolInject<PlayerInputComp> _playerInput = default;
        private readonly EcsPoolInject<RpgComp> _rpg = default;
        private readonly EcsPoolInject<AbilityComp> _ability = default;
        private readonly EcsPoolInject<HasAbilities> _hasAbilitiesPool = default;

        private readonly EcsCustomInject<SceneData> _sc = default;
        private readonly EcsCustomInject<Configuration> _cf = default;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var playerEntity = world.NewEntity();

            ref var player = ref _playerPool.Value.Add(playerEntity);
            ref var rpg = ref _rpg.Value.Add(playerEntity);
            _playerInput.Value.Add(playerEntity);

            var playerPrefab = _cf.Value.playerConfiguration.playerPrefab;
            var playerFollowCameraPrefab = _cf.Value.playerConfiguration.playerFollowCameraPrefab;
            var playerStartPosition = _sc.Value.playerSpawnPosition.position;
            var playerView = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
            var playerFollowCameraView =
                Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);

            player.Transform = playerView.transform;
            player.Position = playerStartPosition;
            player.Rotation = Quaternion.identity;
            player.CharacterController = playerView.GetComponent<CharacterController>();
            player.PlayerCameraRoot = playerView.GetComponentInChildren<PlayerCameraRootView>().transform;
            player.Grounded = true;
            player.PlayerCamera = playerFollowCameraView;

            playerFollowCameraView.Follow = player.PlayerCameraRoot;

            rpg.Health = _cf.Value.playerConfiguration.health;
            rpg.Stamina = _cf.Value.playerConfiguration.stamina;
            rpg.Mana = _cf.Value.playerConfiguration.mana;
            rpg.CanRun = true;
            rpg.CanDash = true;
            rpg.CanJump = true;
            
            CreateAbilities(playerEntity ,world);
        }

        private void CreateAbilities(int playerEntity ,EcsWorld world)
        {
            ref var hasAbilities = ref _hasAbilitiesPool.Value.Add(playerEntity);
            foreach (var ability in _cf.Value.abilityConfiguration.abilityDatas)
            {
                if (ability.name == Idents.Abilities.FireBall)
                {
                    var abilityEntity = world.NewEntity();
                    ref var abil = ref _ability.Value.Add(abilityEntity);
                    abil.Name = ability.name;
                    abil.Damage = ability.damage;
                    abil.LifeTime = ability.lifeTime;
                    abil.Radius = ability.radius;
                    abil.Speed = ability.speed;
                    abil.CostPoint = ability.costPoint;
                    hasAbilities.Entities.Add(abilityEntity);
                }
            }
        }
    }
}