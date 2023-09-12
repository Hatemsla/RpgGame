using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class PlayerCameraRotateSystem : IEcsRunSystem
{
    private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _unitsMove = default;

    private readonly EcsCustomInject<Configuration> _cf = default;
    
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    
    private const float Threshold = 0.01f;
    
    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _unitsMove.Value)
        {
            ref var player = ref _unitsMove.Pools.Inc1.Get(entity);
            ref var input = ref _unitsMove.Pools.Inc2.Get(entity);
            
            if (input.Look.sqrMagnitude >= Threshold)
            {
                var deltaTimeMultiplier = 1f;

                _cinemachineTargetYaw += input.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += input.Look.y * deltaTimeMultiplier;
            }
            
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cf.Value.bottomClamp, _cf.Value.topClamp);

            player.PlayerCameraRoot.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cf.Value.cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }
    }
    
    private float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}