using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class PlayerInitSystem : IEcsInitSystem
{
    private readonly EcsPoolInject<PlayerComp> _playerPool = default;
    private readonly EcsPoolInject<PlayerInputComp> _playerInput = default;

    private readonly EcsCustomInject<SceneData> _sc = default;
    private readonly EcsCustomInject<Configuration> _cf = default;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        var playerEntity = world.NewEntity();

        ref var player = ref _playerPool.Value.Add(playerEntity);
        _playerInput.Value.Add(playerEntity);

        var playerPrefab = _cf.Value.playerPrefab;
        var playerFollowCameraPrefab = _cf.Value.playerFollowCameraPrefab;
        var playerStartPosition = _sc.Value.playerSpawnPosition.position; 
        var playerView = Object.Instantiate(playerPrefab, playerStartPosition, Quaternion.identity);
        var playerFollowCameraView = Object.Instantiate(playerFollowCameraPrefab, Vector3.zero, Quaternion.identity);
        
        player.Transform = playerView.transform;
        player.Position = playerStartPosition;
        player.Rotation = Quaternion.identity;
        player.CharacterController = playerView.GetComponent<CharacterController>();
        player.PlayerCameraRoot = playerView.GetComponentInChildren<PlayerCameraRootView>().transform;
        player.Grounded = true;
        player.PlayerCamera = playerFollowCameraView;
        
        playerFollowCameraView.Follow = player.PlayerCameraRoot;
        
    }
}