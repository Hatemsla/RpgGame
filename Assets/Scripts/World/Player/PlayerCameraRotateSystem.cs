using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using World.Configurations;
using World.RPG;

namespace World.Player
{
    public sealed class PlayerCameraRotateSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp, RpgComp>> _unitsMove = default;

        private readonly EcsCustomInject<Configuration> _cf = default;
        private readonly EcsCustomInject<TimeService> _ts = default;
        private readonly EcsCustomInject<CursorService> _cs = default;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private const float Threshold = 0.01f;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _unitsMove.Value)
            {
                ref var playerComp = ref _unitsMove.Pools.Inc1.Get(entity);
                ref var inputComp = ref _unitsMove.Pools.Inc2.Get(entity);
                ref var rpgComp = ref _unitsMove.Pools.Inc3.Get(entity);
                
                if(!playerComp.CanMove) return;
                
                if (inputComp.Look.sqrMagnitude >= Threshold)
                {
                    var deltaTimeMultiplier = 1f;

                    _cinemachineTargetYaw += inputComp.Look.x * deltaTimeMultiplier;
                    _cinemachineTargetPitch += inputComp.Look.y * deltaTimeMultiplier;
                }

                _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cf.Value.playerConfiguration.bottomClamp,
                    _cf.Value.playerConfiguration.topClamp);
                
                playerComp.PlayerCameraRootTransform.transform.rotation = Quaternion.Euler(
                    _cinemachineTargetPitch + _cf.Value.playerConfiguration.cameraAngleOverride,
                    _cinemachineTargetYaw, 0f);

                if (!inputComp.FreeLook && !_cs.Value.CursorVisible && !rpgComp.IsDead && inputComp.Move == Vector2.zero && !playerComp.IsPose)
                {
                    var desiredYRotation = playerComp.PlayerCameraRootTransform.eulerAngles.y;
                    var currentYRotation = playerComp.Transform.rotation.eulerAngles.y;
                   
                    var newYRotation = Mathf.LerpAngle(
                        currentYRotation, desiredYRotation,  _ts.Value.DeltaTime * _cf.Value.playerConfiguration.rotationSpeed);
                    
                    playerComp.Transform.rotation = Quaternion.Euler(0f, newYRotation, 0f);
                }
            }
        }

        private float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}