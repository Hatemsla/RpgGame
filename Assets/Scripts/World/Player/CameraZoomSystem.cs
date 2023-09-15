using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public class CameraZoomSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _unitsMove = default;
        private readonly EcsCustomInject<Configuration> _config=default;
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _unitsMove.Value)
            {
                ref var player = ref _unitsMove.Pools.Inc1.Get(entity);
                ref var input = ref _unitsMove.Pools.Inc2.Get(entity);

                var cameraDistance = player.PlayerCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

                if (input.Zoom != 0)
                {
                    cameraDistance.CameraDistance -= input.Zoom * _config.Value.playerConfiguration.zoomSpeed;
                    cameraDistance.CameraDistance = Mathf.Clamp(cameraDistance.CameraDistance,
                        _config.Value.playerConfiguration.minZoomDistance, _config.Value.playerConfiguration.maxZoomDistance);
                }
            }
        }
    }
}