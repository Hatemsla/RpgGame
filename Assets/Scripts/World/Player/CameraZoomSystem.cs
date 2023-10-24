using Cinemachine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;

namespace World.Player
{
    public class CameraZoomSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _unitsMove = default;
        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<CursorService> _cs = default;

        private Cinemachine3rdPersonFollow _camera3rdPerson;
        
        public void Init(IEcsSystems systems)
        {
            foreach (var entity in _unitsMove.Value)
            {
                ref var player = ref _unitsMove.Pools.Inc1.Get(entity);

                _camera3rdPerson = player.PlayerCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            }
        }
            
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _unitsMove.Value)
            {
                ref var input = ref _unitsMove.Pools.Inc2.Get(entity);

                if (_cs.Value.CursorVisible) return;

                if (input.Zoom != 0)
                {
                    _camera3rdPerson.CameraDistance -= input.Zoom * _cf.Value.playerConfiguration.zoomSpeed;
                    _camera3rdPerson.CameraDistance = Mathf.Clamp(_camera3rdPerson.CameraDistance,
                        _cf.Value.playerConfiguration.minZoomDistance, _cf.Value.playerConfiguration.maxZoomDistance);
                }
            }
        }
    }
}