using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

public sealed class PlayerInitSystem : IEcsInitSystem
{
    private readonly EcsPoolInject<Unit> _unitPool = default;
    private readonly EcsPoolInject<ControllerByPlayer> _controllerByPlayer = default;

    public void Init(IEcsSystems systems)
    {
        var _filter = systems.GetWorld().Filter<ControllerByPlayer>();
    }
}