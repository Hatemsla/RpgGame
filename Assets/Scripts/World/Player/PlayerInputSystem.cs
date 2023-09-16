using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace World.Player
{
    public sealed class PlayerInputSystem : IEcsRunSystem, IEcsInitSystem
    {
        private readonly EcsFilterInject<Inc<PlayerComp, PlayerInputComp>> _players = default;
        private readonly EcsCustomInject<MainInput> _mn = default;

        public void Init(IEcsSystems systems)
        {
            _mn.Value.Player.Enable();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _players.Value)
            {
                ref var inputComp = ref _players.Pools.Inc2.Get(entity);
                var input = _mn.Value.Player;

                inputComp.Move = input.Move.ReadValue<Vector2>();
                inputComp.Look = input.Look.ReadValue<Vector2>();
                inputComp.Jump = input.Jump.IsPressed();
                inputComp.Sprint = input.Sprint.IsPressed();
                inputComp.Zoom = input.Zoom.ReadValue<Vector2>().y; // вращение колесика по y
                inputComp.Walk = input.Walk.WasPerformedThisFrame();
            }
        }
    }
}