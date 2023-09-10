using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

public sealed class PlayerInputSystem : IEcsRunSystem
{
    private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _units = default;
    private readonly EcsCustomInject<MainInput> _mn = default;
    
    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _units.Value)
        {
            ref var inputComp = ref _units.Pools.Inc2.Get(entity);
            var input = _mn.Value.Player;

            // Понять как сделать через InputSystem, текущий вариант почему то не работает, значения не считываются
            // inputComp.Move = _mainInput.Player.Move.ReadValue<Vector2>();
            // inputComp.Look = input.Look.ReadValue<Vector2>();
            // inputComp.Jump = input.Jump.ReadValue<bool>();
            // inputComp.Sprint = input.Sprint.ReadValue<bool>();

            inputComp.Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            inputComp.Look = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

            inputComp.Jump = Input.GetKeyUp(KeyCode.Space);
            inputComp.Sprint = Input.GetKey(KeyCode.LeftShift);
        }
    }
}