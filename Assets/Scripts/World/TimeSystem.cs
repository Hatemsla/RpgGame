using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class TimeSystem : IEcsRunSystem
{
    private readonly EcsCustomInject<TimeService> _ts = default;
    
    public void Run(IEcsSystems systems)
    {
        _ts.Value.Time = Time.time;
        _ts.Value.UnscaledTime = Time.unscaledTime;
        _ts.Value.UnscaledDeltaTime = Time.unscaledDeltaTime;
        _ts.Value.DeltaTime = Time.deltaTime;
    }
}